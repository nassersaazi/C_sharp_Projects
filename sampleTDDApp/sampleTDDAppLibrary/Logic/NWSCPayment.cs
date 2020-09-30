using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sampleTDDAppLibrary.Logic
{
    public class NWSCPayment : Payment,IPayment
    {
        public PostResponse pay(ITransaction trans)
        {
           
            if (trans.CustomerTel == null)
            {
                trans.CustomerTel = "";
            }
            if (trans.Email == null)
            {
                trans.Email = "";
            }
            string vendorCode = trans.VendorCode;
           

            if (CheckForEmptyProperties((NWSCTransaction)trans))
            {
                try
                {

                    if (!IsValidReversalStatus(trans))
                    {
                        resp.HandleResponse(trans, resp, "25", "");
                        return resp;
                    }

                    if (!bll.IsNumeric(trans.TransactionAmount))
                    {
                        resp.HandleResponse(trans, resp, "3", "");
                        return resp;
                    }
                    if (!bll.IsValidDate(trans.PaymentDate))
                    {
                        resp.HandleResponse(trans, resp, "4", "");
                        return resp;
                    }
                    DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                    if (!isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                    {
                        resp.HandleResponse(trans, resp, "2", "");
                        return resp;
                    }
                    if (!isActiveVendor(trans.VendorCode, vendaData))
                    {
                        resp.HandleResponse(trans, resp, "11", "");
                        return resp;
                    }
                    if (!isSignatureValid(trans))
                    {
                        resp.HandleResponse(trans, resp, "18", "");
                        return resp;
                    }
                    if (!pv.PhoneNumbersOk(trans.CustomerTel))
                    {
                        resp.HandleResponse(trans, resp, "12", "");
                        return resp;
                    }
                    if (IsduplicateVendorRef(trans))
                    {
                        resp.HandleResponse(trans, resp, "20", "");
                        return resp;
                                                    
                    }
                    if (IsduplicateCustPayment(trans))
                    {
                        resp.HandleResponse(trans, resp, "21", "");
                        return resp;
                    }

                    trans.Reversal = GetReversalState(trans);

                    if (!HasOriginalEntry(trans))
                    {
                        resp.HandleResponse(trans, resp, "24", "");
                        return resp;
                    }
                    if (!ReverseAmountsMatch(trans))
                    {
                        resp.HandleResponse(trans, resp, "26", "");

                    }
                    else
                    {
                        if (dp.IsChequeBlacklisted(trans))
                        {
                            resp.HandleResponse(trans, resp, "29", "");

                        }
                        else
                        {

                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                            if ((vendorType.Equals("PREPAID")))
                            {
                                resp.HandleResponse(trans, resp, "29", "NOT ENABLED FOR PREPAID VENDORS");


                            }
                            else
                            {
                                UtilityCredentials creds = dp.GetUtilityCreds("NWSC", trans.VendorCode);
                                if (creds.UtilityCode.Equals(""))
                                {
                                    resp.HandleResponse(trans, resp, "29", "");


                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(trans.CustomerType))
                                    {
                                        trans.CustomerType = "";
                                    }
                                    resp.HandleResponse(trans, resp, "0", "");
                                }

                            }
                        }
                    }
                       
                    if (resp.StatusCode.Equals("2"))
                    {
                        DataTable dt = dp.GetVendorDetails(vendorCode);
                        if (dt.Rows.Count != 0)
                        {
                            string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                            int loginCount = int.Parse(strLoginCount);
                            loginCount = loginCount + 1;
                            if (loginCount == 3)
                            {
                                dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                                dp.DeactivateVendor(vendorCode, ipAddress);
                            }
                            {
                                dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                            }
                        }
                    }
                }
                catch (System.Net.WebException wex)
                {

                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                    dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "NWSC");
                }
                catch (SqlException sqlex)
                {
                    resp.StatusCode = "31";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
                    resp.PegPayPostId = "";
                }
                catch (Exception ex)
                {
                    resp.StatusCode = "32";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
                    resp.PegPayPostId = "";
                    dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "NWSC");
                } 
            }
            return resp;
        }

        private bool CheckForEmptyProperties(NWSCTransaction trans)
        {
            if (trans.CustName == null || trans.CustName.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "13", "");
                return false;
            }

            else if (trans.Area == null || trans.Area.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "35", "");
                return false;

            }

            else if (trans.TransactionType == null || trans.TransactionType.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "14", "");
                return false;
            }

            else if (trans.VendorTransactionRef == null || trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "16", "");
                return false;
            }

            else if (trans.Teller == null || trans.Teller.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "17", "");
                return false;
            }

            else if (trans.DigitalSignature == null || trans.DigitalSignature.Length == 0)
            {
                resp.HandleResponse(trans, resp, "19", "");
                return false;
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.HandleResponse(trans, resp, "22", "");
                return false;
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.HandleResponse(trans, resp, "22", "");
                return false;
            }
            else if ((trans.Reversal == "1" && trans.Narration == null) || (trans.Reversal == "1" && trans.Narration.Equals("")))
            {
                resp.HandleResponse(trans, resp, "23", "");
                return false;
            }

            return true;
        }




        public override bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData)
         {
            Console.WriteLine("IsValidVendorCredentials from child class\n");

            return true;
        }
    }
}
