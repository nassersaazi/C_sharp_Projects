using kcbTester.EntityObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace kcbTester.ControlObjects
{
    public class BusinessLogic
    {
        //public PostResponse MakeStartTimesPayment(Transaction trans)
        //{
        //    PostResponse resp = new PostResponse();
        //    DatabaseHandler dp = new DatabaseHandler();
        //    BusinessLogic bll = new BusinessLogic();
        //    PhoneValidator pv = new PhoneValidator();
        //    if (trans.CustomerTel == null)
        //    {
        //        trans.CustomerTel = "";
        //    }
        //    if (trans.Email == null)
        //    {
        //        trans.Email = "";
        //    }
        //    List<string> payCategories = new List<string>();
        //    payCategories.Add("RECHARGE");
        //    payCategories.Add("PAYMENT");
        //    string vendorCode = trans.VendorCode;
        //    try
        //    {
        //        dp.SaveRequestlog(trans.VendorCode, "STARTIMES", "POSTING", trans.CustRef, trans.Password);
        //        if (string.IsNullOrEmpty(trans.CustName))
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "13";
        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
        //        else if (string.IsNullOrEmpty(trans.Area.Trim()))
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "35";
        //            resp.StatusDescription = "PLEASE PROVIDE BOUQUET";
        //        }
        //        else if (string.IsNullOrEmpty(trans.TransactionType))
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "14";
        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
               
        //        else if (string.IsNullOrEmpty(trans.Teller))
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "17";

        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
                
                
        //        else if (!IsValidReversalStatus(trans))
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "25";
        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
        //        else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "22";
        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
        //        else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "22";
        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
        //        else if (trans.Reversal == "1" && trans.Narration == null)
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "23";
        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
        //        else if (trans.Reversal == "1" && trans.Narration.Equals(""))
        //        {
        //            resp.PegPayPostId = "";
        //            resp.StatusCode = "23";
        //            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        }
               
        //        else
        //        {
        //            if (IsNumeric(trans.TransactionAmount))
        //            {
        //                if (bll.IsValidDate(trans.PaymentDate))
        //                {
        //                    DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
        //                    if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
        //                    {
        //                        if (isActiveVendor(trans.VendorCode, vendaData))
        //                        {
        //                            if (isSignatureValid(trans))
        //                            {
        //                                if (pv.PhoneNumbersOk(trans.CustomerTel))
        //                                {
        //                                    if (!IsduplicateVendorRef(trans))
        //                                    {
        //                                        if (!IsduplicateCustPayment(trans))
        //                                        {
        //                                            trans.Reversal = trans.Tin.Equals("RECHARGE") ? "0" : GetReversalState(trans);
        //                                            if (HasOriginalEntry(trans))
        //                                            {
        //                                                if (ReverseAmountsMatch(trans))
        //                                                {
        //                                                    if (!IsChequeBlacklisted(trans))
        //                                                    {

        //                                                        string vendorType = vendaData.Rows[0]["VendorType"].ToString();
        //                                                        if (!(vendorType.Equals("PREPAID")))
        //                                                        {
        //                                                            UtilityCredentials creds = dp.GetUtilityCreds("STARTIMES", trans.VendorCode);
        //                                                            if (!creds.UtilityCode.Equals(""))
        //                                                            {
        //                                                                if (string.IsNullOrEmpty(trans.CustomerType))
        //                                                                {
        //                                                                    trans.CustomerType = "";
        //                                                                }

        //                                                                resp.PegPayPostId = dp.PostPayTvTransaction(trans, "STARTIMES");
        //                                                                resp.StatusCode = "0";
        //                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                                            }
        //                                                            else
        //                                                            {
        //                                                                resp.PegPayPostId = "";
        //                                                                resp.StatusCode = "29";
        //                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                                            }
        //                                                        }
        //                                                        else
        //                                                        {
        //                                                            resp.PegPayPostId = "";
        //                                                            resp.StatusCode = "29";
        //                                                            resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
        //                                                        }
        //                                                    }
        //                                                    else
        //                                                    {
        //                                                        resp.PegPayPostId = "";
        //                                                        resp.StatusCode = "29";
        //                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                                    }
        //                                                }
        //                                                else
        //                                                {
        //                                                    resp.PegPayPostId = "";
        //                                                    resp.StatusCode = "26";
        //                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                                }
        //                                            }
        //                                            else
        //                                            {
        //                                                resp.PegPayPostId = "";
        //                                                resp.StatusCode = "24";
        //                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                            }

        //                                        }
        //                                        else
        //                                        {
        //                                            resp.PegPayPostId = "";
        //                                            resp.StatusCode = "21";
        //                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        resp.PegPayPostId = "";
        //                                        resp.StatusCode = "20";
        //                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    resp.PegPayPostId = "";
        //                                    resp.StatusCode = "12";
        //                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                resp.PegPayPostId = "";
        //                                resp.StatusCode = "18";
        //                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            resp.PegPayPostId = "";
        //                            resp.StatusCode = "11";
        //                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        resp.PegPayPostId = "";
        //                        resp.StatusCode = "2";
        //                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                    }
        //                }
        //                else
        //                {
        //                    resp.PegPayPostId = "";
        //                    resp.StatusCode = "4";
        //                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //                }
        //            }
        //            else
        //            {
        //                resp.PegPayPostId = "";
        //                resp.StatusCode = "3";
        //                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //            }
        //        }
        //        if (resp.StatusCode.Equals("2"))
        //        {
        //            DataTable dt = dp.GetVendorDetails(vendorCode);
        //            if (dt.Rows.Count != 0)
        //            {
        //                string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //                string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
        //                int loginCount = int.Parse(strLoginCount);
        //                loginCount = loginCount + 1;
        //                if (loginCount == 3)
        //                {
        //                    dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
        //                    dp.DeactivateVendor(vendorCode, ipAddress);
        //                }
        //                {
        //                    dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
        //                }
        //            }
        //        }
        //    }
        //    catch (System.Net.WebException wex)
        //    {
        //        resp.StatusCode = "0";
        //        resp.StatusDescription = "SUCCESS";
        //        dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "STARTIMES");
        //    }
        //    catch (SqlException sqlex)
        //    {
        //        resp.StatusCode = "31";
        //        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
        //        resp.PegPayPostId = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        resp.StatusCode = "32";
        //        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        //        dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
        //        resp.PegPayPostId = "";
        //        dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "STARTIMES");
        //    }
        //    return resp;
        //}

        public void ValidateCustomer(Transaction tran)
        {

            Console.WriteLine("************************************************************");
            Console.WriteLine("Validating student with student Id : " + tran.CustomerRef);
      

            
            string api_request_xml_url = "http://localhost:8599/TestKcbVas/VasApi.asmx?WSDL";
            string username = tran.Username;
            string password = tran.Password;
            string area = tran.Area;
            string serviceCode = tran.ServiceCode;
            string customerRef = tran.CustomerRef;


            string flex_soap_action = "";


            string kcbApiRequest = @"<Envelope xmlns = 'http://schemas.xmlsoap.org/soap/envelope/'>
                                     <Body>
                                        <ValidateCustomer xmlns = 'http://pegasustechnologies.co.ug/'>
                                            <!--Optional-->
                                            <requestData>
                                                <!--Optional-->
                                                <Credentials>
                                                    <Username>" + username + @"</Username>
                                                    <Password>" + password + @"</Password>
                                                </Credentials>
                                                <!--Optional-->
                                                <PayLoad>
                                                    <Area>" + area + @"</Area>
                                                    <ServiceCode>" + serviceCode + @"</ServiceCode>
                                                    <CustomerRef>" + customerRef + @"</CustomerRef>
                                                </PayLoad>
                                            </requestData>
                                        </ValidateCustomer>
                                     </Body>
                                     </Envelope>";

            string flex_resp = SendRequest(api_request_xml_url, flex_soap_action, kcbApiRequest);
            if (String.IsNullOrEmpty(flex_resp))
            {
                Console.WriteLine("\nEmpty response from Utility!!!\n");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================================== Response : ValidateCustomer ======================================" + "\n");
            Console.WriteLine("\n" + FormatXml(flex_resp));
            Console.ResetColor();
        }

        public void PostTransaction(Transaction tran)
        {

            Console.WriteLine("************************************************************");
            Console.WriteLine("Posting transaction with Beneficiary ID : " + tran.CustomerRef);



            string api_request_xml_url = "http://localhost:8599/TestKcbVas/VasApi.asmx?WSDL";
            string username = tran.Username;
            string password = tran.Password;
            string area = tran.Area;
            string serviceCode = tran.ServiceCode;
            string customerRef = tran.CustomerRef;


            string flex_soap_action = "";


            string kcbApiRequest = @"<Envelope xmlns = 'http://schemas.xmlsoap.org/soap/envelope/'>
                                     <Body>
                                        <PostTransaction xmlns='http://pegasustechnologies.co.ug/'>
                                          <!--Optional-->
                                          <requestData>
                                            <!--Optional-->
                                            <Credentials>
                                              <Username>KCB</Username>
                                              <Password>63T25KG001</Password>
                                               </Credentials>
                                               <!--Optional-->
                                               <PayLoad>
                                                 <Area>KCBJ</Area>
                                                 <BeneficiaryID>05 - 01777</BeneficiaryID>
                                                 <BeneficiaryName>Bogere Fareek</BeneficiaryName>
                                                    <Narration>FEES PAYMENT</Narration>
                                                       <MerchantID>69</MerchantID>
                                                       <ReferenceID>4gjh05656342</ReferenceID>
                                                          <Amount>100</Amount>
                                                          <Currency>UGX</Currency>
                                                          <MSISDN>256777853085</MSISDN>
                                                          <Addendum></Addendum>
            
                                                        </ PayLoad >
            
                                                      </ requestData >
            
                                                    </ PostTransaction >
            
                                                                                     </Body>
                                                                         </Envelope>";

            string flex_resp = SendRequest(api_request_xml_url, flex_soap_action, kcbApiRequest);
            if (String.IsNullOrEmpty(flex_resp))
            {
                Console.WriteLine("\nEmpty response from Utlity!!!\n");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("================================== Response : PostTransaction ======================================" + "\n");
            Console.WriteLine( FormatXml(flex_resp) + "\n" );
            Console.ResetColor();
        }

        private bool IsValidReversalStatus(Transaction trans)
        {
            if (trans.Reversal == null)
            {
                return false;
            }
            else
            {
                if (trans.Reversal.Equals("0") || trans.Reversal.Equals("1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsNumeric(string amount)
        {

            if (amount.Equals("0"))
            {
                return false;
            }
            else
            {
                double amt = double.Parse(amount);
                amount = amt.ToString();
                float Result;
                return float.TryParse(amount, out Result);
            }
        }

        public bool IsValidDate(string paymentDate)
        {
            DateTime date;
            //if (DateTime.TryParse(paymentDate, out date))
            //{
            //return true;
            string format = "dd/MM/yyyy";
            return DateTime.TryParseExact(paymentDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            //}
            //else
            //{
            //    return false;
            //}
        }

        string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                // Handle and throw if fatal exception here; don't just ignore them
                return xml;
            }
        }

        #region ... 01: SendRequest
        public static string SendRequest(string url, string soap_Action, string data)
        {
            string resp = "";

            try
            {
                //HttpWebRequest request = CreateSOAPWebRequest(url, soap_Action);
                HttpWebRequest request = CreateSOAPWebRequest(url);
                XmlDocument SOAPReqBody = new XmlDocument();
                SOAPReqBody.LoadXml(data);
                using (Stream stream = request.GetRequestStream())
                {
                    SOAPReqBody.Save(stream);
                }
                using (WebResponse Serviceres = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                    {
                        var ServiceResult = rd.ReadToEnd();
                        resp = ServiceResult;
                    }
                }
            }
            catch (Exception mm)
            {
                resp = mm.Message;
            }

            return resp;
        }

        #endregion

        #region ... 02 Request Headers
        public static HttpWebRequest CreateSOAPWebRequest(string url)
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(url);
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            Req.KeepAlive = true;
            Req.Method = "POST";
            return Req;
        }
        #endregion

    }
}

