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
    public class DSTVPayment : Payment,IPayment
    {
        public PostResponse pay(ITransaction trans)
        {   
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            BusinessLogic bll = new BusinessLogic();
            PhoneValidator pv = new PhoneValidator();
            if (trans.CustomerTel == null)
            {
                trans.CustomerTel = "";
            }
            if (trans.Email == null)
            {
                trans.Email = "";
            }
            string vendorCode = trans.VendorCode;
            try
            {
                dp.SaveRequestlog(trans.VendorCode, "DSTV", "POSTING", trans.CustRef, trans.Password);
                if (trans.CustName == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "13";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.CustName.Trim().Equals(""))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "13";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.TransactionType == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "14";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.TransactionType.Trim().Equals(""))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "14";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.VendorTransactionRef == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "16";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.VendorTransactionRef.Trim().Equals(""))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "16";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.Teller == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "17";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }

                else if (trans.Teller.Trim().Equals(""))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "17";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.CustRef == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "1";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }

                else if (trans.CustRef.Trim().Equals(""))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "1";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.DigitalSignature == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "19";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.DigitalSignature.Length == 0)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "19";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (!IsValidReversalStatus(trans))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "25";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "22";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "22";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.Reversal == "1" && trans.Narration == null)
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "23";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (trans.Reversal == "1" && trans.Narration.Equals(""))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "23";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                else if (string.IsNullOrEmpty(trans.Area))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "23";
                    resp.StatusDescription = "PLEASE SUPPLY A BOUQUET CODE";
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
                                                        if (ReverseAmountsMatch(trans))
                                                        {
                                                            if (!dp.IsChequeBlacklisted(trans))
                                                            {
                                                                string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                                if (!(vendorType.Equals("PREPAID")))
                                                                {
                                                                    UtilityCredentials creds = dp.GetUtilityCreds("DSTV", trans.VendorCode);
                                                                    creds.UtilityCode = "DSTV";

                                                                    if (!creds.UtilityCode.Equals(""))
                                                                    {
                                                                        resp.PegPayPostId = dp.PostPayTvTransaction((DSTVTransaction)trans, trans.UtilityCode);
                                                                        resp.StatusCode = "0";
                                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                    }
                                                                    else
                                                                    {
                                                                        resp.PegPayPostId = "";
                                                                        resp.StatusCode = "29";
                                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "26";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "24";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }

                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "21";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }
                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "20";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "12";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "18";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "11";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "2";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "4";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "3";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
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
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "URA");
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
                dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "URA");
            }
            return resp;
        }

    }
}
