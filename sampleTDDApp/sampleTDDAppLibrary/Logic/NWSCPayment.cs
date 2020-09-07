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
    public class NWSCPayment : Payment,ITransactionValidity
    {
        public override PostResponse pay(NWSCTransaction trans)
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
           

            if (CheckForEmptyProperties(trans))
            {
                try
                {

                    if (!IsValidReversalStatus(trans))
                    {
                        resp.HandleResponse(trans, resp, "25", "");
                    }

                    else
                    {
                        if (bll.IsNumeric(trans.TransactionAmount))
                        {
                            if (bll.IsValidDate(trans.PaymentDate))
                            {
                                DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                                if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                                {
                                    if (isActiveVendor(trans.VendorCode, vendaData))
                                    {
                                        if (isSignatureValid(trans))
                                        {
                                            if (pv.PhoneNumbersOk(trans.CustomerTel))
                                            {
                                                if (!IsduplicateVendorRef(trans))
                                                {
                                                    if (!IsduplicateCustPayment(trans))
                                                    {
                                                        trans.Reversal = GetReversalState(trans);
                                                        if (HasOriginalEntry(trans))
                                                        {
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
                                                        }
                                                        else
                                                        {
                                                            resp.HandleResponse(trans, resp, "24", "");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        resp.HandleResponse(trans, resp, "21", "");
                                                    }
                                                }
                                                else
                                                {
                                                    resp.HandleResponse(trans, resp, "20", "");
                                                }
                                            }
                                            else
                                            {
                                                resp.HandleResponse(trans, resp, "12", "");
                                            }
                                        }
                                        else
                                        {
                                            resp.HandleResponse(trans, resp, "18", "");
                                        }
                                    }
                                    else
                                    {
                                        resp.HandleResponse(trans, resp, "11", "");
                                    }
                                }
                                else
                                {
                                    resp.HandleResponse(trans, resp, "2", "");
                                }
                            }
                            else
                            {
                                resp.HandleResponse(trans, resp, "4", "");

                            }
                        }
                        else
                        {
                            resp.HandleResponse(trans, resp, "3", "");
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

        public override bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData)
        {
            Console.WriteLine("IsValidVendorCredentials from child class\n");

            return true;
        }
    }
}
