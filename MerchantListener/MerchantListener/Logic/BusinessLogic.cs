using MerchantListener.EntityObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MerchantListener.MerchantApi;
using System.Data;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Reflection;

namespace MerchantListener.Logic
{
    class BusinessLogic
    {
        DatabaseHandler dbh = new DatabaseHandler();
        Merchant mapi = new Merchant();
       SearchFilters mapiRequest = new SearchFilters();
        MerchantTransaction mtran = new MerchantTransaction();
        MerchantApi.Response apiResp = new MerchantApi.Response();
        MerchantApi.Response payResp = new MerchantApi.Response();
        internal string apiUsername = "ussd";
        internal string apiPassword = "ussd298";
        public string GetRequestIp()
        {
            string _custIP = null;
            try
            {

                _custIP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];

                if (String.IsNullOrEmpty(_custIP))
                {
                    _custIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                }
                if (String.IsNullOrEmpty(_custIP))
                {
                    _custIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                string method = methodBase.DeclaringType.Name + "." + methodBase.Name;
                LogError("", method, "", "Exception", ex.Message, "");
                _custIP = GetServerIpIpValue();
            }
            return _custIP;
        }

        public string ProcessRequest(string request, string sourceip,string serverip)
        {
            ListenerResponse valResponse = new ListenerResponse();
            string xmlResponse = "";
            try
            {
               
                string RequestType = GetTranRequestType(request);
                if (RequestType.Equals("ValidateMerchant"))
                {
                    Request vmreq = GetValidateMerchantRequestDetails(request);
                    if (vmreq.StatusCode.Equals("0"))
                    {
                        xmlResponse = GetMerchantDetailsApi(vmreq);

                    }
                    else
                    {
                        LogError(vmreq.Username, "ProcessThread->GetMerchantDetailsApi", vmreq.MerchantCode, "Logical", vmreq.StatusDescription, vmreq.RequestId);
                    }

                    LogRequestResponse(request, xmlResponse, vmreq.RequestId, "ValidateMerchant", vmreq.Username, sourceip, serverip);
                }
                else if (RequestType.Equals("MakeMerchantPayment"))
                {
                    Request vmreq = GetMerchantPaymentDetails(request);
                    if (vmreq.StatusCode.Equals("0"))
                    {
                        xmlResponse = GetMerchantPaymentDetailsApi(vmreq);

                    }
                    else
                    {
                        LogError(vmreq.Username, "ProcessThread->GetMerchantPaymentDetailsApi", vmreq.MerchantCode, "Logical", vmreq.StatusDescription, vmreq.RequestId);
                    }


                    LogRequestResponse(request, xmlResponse, vmreq.RequestId, "MakeMerchantPayment", vmreq.Username, sourceip, serverip);
                }
                else if (RequestType.Equals("GetMerchantCharge"))
                {
                    Request vmreq = GetMerchantChargeRequestDetails(request);
                    if (vmreq.StatusCode.Equals("0"))
                    {
                        xmlResponse = GetMerchantChargeDetailsApi(vmreq);

                    }
                    else
                    {
                        LogError(vmreq.Username, "ProcessThread->GetMerchantChargeDetailsApi", vmreq.MerchantCode, "Logical", vmreq.StatusDescription, vmreq.RequestId);
                    }


                    LogRequestResponse(request, xmlResponse, vmreq.RequestId, "GetMerchantCharge", vmreq.Username, sourceip, serverip);
                }
                else if (RequestType.Equals("GetTransactionStatus"))
                {

                    Request vmreq = GetMerchantTranStatusDetails(request);
                    if (vmreq.StatusCode.Equals("0"))
                    {
                        xmlResponse = GetMerchantTransactionDetailsApi(vmreq);

                    }
                    else
                    {
                        LogError(vmreq.Username, "ProcessThread->GetMerchantTransactionDetailsApi", vmreq.MerchantCode, "Logical", vmreq.StatusDescription, vmreq.RequestId);
                    }

                    LogRequestResponse(request, xmlResponse, vmreq.RequestId, "GetTransactionStatus", vmreq.Username, sourceip, serverip);
                }
                else if (RequestType.Equals("ValidateCustomerRef"))
                {

                    Request vmreq = GetValidateCustRefDetails(request);
                    if (vmreq.StatusCode.Equals("0"))
                    {
                        xmlResponse = GetValidateCustRefDetailsApi(vmreq);

                    }
                    else
                    {
                        LogError(vmreq.Username, "ProcessThread->GetValidateCustRefDetailsApi", vmreq.MerchantCode, "Logical", vmreq.StatusDescription, vmreq.RequestId);
                    }

                    LogRequestResponse(request, xmlResponse, vmreq.RequestId, "ValidateCustomerRef", vmreq.Username, sourceip, serverip);
                }

              
               
            }
            catch (Exception ex)
            {
                valResponse.StatusCode = "200";
                valResponse.StatusDescription = ex.Message;
               LogError("", "ProcessThread", "", "Exception", ex.Message, "");
            }

            return xmlResponse;
        }

        public string Serialize(object dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (StringWriter stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }

        public string  GetMerchantDetailsApi(Request vmRequest)
        {
            ListenerResponse resp = new ListenerResponse();
            string xmlResponse = "";
            mapiRequest.ReferenceId = vmRequest.MerchantCode;
            mapiRequest.MerchantId = vmRequest.MerchantCode;
            mapiRequest.RequestAgent = "1";
            mapiRequest.Channel = vmRequest.Channel;
            mapiRequest.ApiPassword = apiPassword;
            mapiRequest.ApiUsername = apiUsername;

            resp = GetVendorCredentials(vmRequest.Username, vmRequest.Password);
            if(resp.StatusCode.Equals("0") && resp.StatusDescription.Equals("ValidVendor"))
            {
                if (IsValidDigitalSignature(vmRequest))
                {
                    apiResp = mapi.GetMerchantDetails(mapiRequest);
                    xmlResponse = GetValidationXmlResponse(apiResp);
                }
                else
                {
                    apiResp.Status = "30";
                    apiResp.StatusDescription = "Invalid Digital Signature";
                    xmlResponse = GetValidationXmlResponse(apiResp);
                }
            
            }
            else
            {
                apiResp.Status = "100";
                apiResp.StatusDescription = "Invalid Vendor Credentials";
                xmlResponse = GetValidationXmlResponse(apiResp);
            }
           

            return xmlResponse;
        }

        public string GetMerchantTransactionDetailsApi(Request vmRequest)
        {
            ListenerResponse resp = new ListenerResponse();
            MerchantChargeResponse chargeResp = new MerchantChargeResponse();
            string xmlResponse = "";

            resp = GetVendorCredentials(vmRequest.Username, vmRequest.Password);
            if (resp.StatusCode.Equals("0") && resp.StatusDescription.Equals("ValidVendor"))
            {
                if (IsValidDigitalSignature(vmRequest))
                {
                    mapiRequest.ReferenceId = vmRequest.TransactionId;
                    mapiRequest.MerchantId = vmRequest.MerchantCode;
                    mapiRequest.RequestAgent = "1";
                    mapiRequest.ApiUsername = apiUsername;
                    mapiRequest.ApiPassword = apiPassword;
                    mapiRequest.Channel = vmRequest.Channel;
                    mtran = mapi.GetTransactionDetails(mapiRequest);
                    if (!string.IsNullOrEmpty(mtran.Status.ToString()))
                    {

                        if (!(string.IsNullOrEmpty(mtran.Status.ToString())))
                        {

                            xmlResponse = GetMerchantTranStatusXmlResponse(mtran);
                        }
                        else
                        {
                            mtran.Status = "201";
                            mtran.StatusDescription = "Transaction Not Found";
                            xmlResponse = GetMerchantTranStatusXmlResponse(mtran);
                        }

                    }
                    else
                    {
                        mtran.Status = mtran.Status;
                        mtran.StatusDescription = mtran.StatusDescription;
                        xmlResponse = GetMerchantTranStatusXmlResponse(mtran);
                    }

                }
                else
                {
                    apiResp.Status = "30";
                    apiResp.StatusDescription = "Invalid Digital Signature";
                    xmlResponse = GetMerchantPaymentXmlResponse(apiResp);
                }

            }
            else
            {
                apiResp.Status = "100";
                apiResp.StatusDescription = "Invalid Vendor Credentials";
                xmlResponse = GetMerchantPaymentXmlResponse(apiResp);
            }
            return xmlResponse;
        }

        public string GetValidateCustRefDetailsApi(Request vmRequest)
        {
            ListenerResponse resp = new ListenerResponse();
            ValidateCustomerRefResponse apiresponse = new ValidateCustomerRefResponse();
            string xmlResponse = "";

            resp = GetVendorCredentials(vmRequest.Username, vmRequest.Password);
            if (resp.StatusCode.Equals("0") && resp.StatusDescription.Equals("ValidVendor"))
            {
                if (IsValidDigitalSignature(vmRequest))
                {
                    mapiRequest.ReferenceId = vmRequest.MerchantCode;
                    mapiRequest.MerchantId = vmRequest.MerchantCode;
                    mapiRequest.RequestAgent = "1";
                    mapiRequest.ApiUsername = apiUsername;
                    mapiRequest.ApiPassword = apiPassword;
                    mapiRequest.Channel = vmRequest.Channel;
                    Response apiResp2 = mapi.GetMerchantDetails(mapiRequest);
                    bool NotReferenced = false;
                    bool IsReferenced =  bool.TryParse(apiResp2.Customer.CustomerType, out NotReferenced);
                    if (IsReferenced)
                    {
                        if (apiResp2.Status.Equals("0"))
                        {

                            if (vmRequest.MerchantCode.Equals("108633"))
                            {
                                TotalLogic total = new TotalLogic();
                                CardValidationResponse rr = new CardValidationResponse();

                                rr = total.CallWebService(vmRequest.CustomerRef);
                                apiresponse.CustomerName = rr.HolderName;
                                apiresponse.MerchantName = apiResp2.Customer.FullName;
                                apiresponse.StatusCode = rr.StatusCode;
                                apiresponse.StatusDescription = rr.StatusDescription;
                                apiresponse.MerchantCode = vmRequest.MerchantCode;
                                apiresponse.OutstandingBalance = "0";
                                apiresponse.MinimumBalance = "0";
                                xmlResponse = GetValidateCustRefXmlResponse(apiresponse);

                            }
                            else
                            {
                                mapiRequest.ReferenceId = vmRequest.CustomerRef;
                                apiResp2 = mapi.GetMerchantCustomer(mapiRequest);
                                apiresponse.MerchantName = apiResp2.Customer.FullName;
                                apiresponse.CustomerName = apiResp2.Customer.FullName;
                                apiresponse.MerchantCode = vmRequest.MerchantCode;
                                apiresponse.OutstandingBalance = apiResp2.Customer.RunningBalance;
                                apiresponse.StatusCode = apiResp2.Status;
                                apiresponse.StatusDescription = apiResp2.StatusDescription;
                                xmlResponse = GetValidateCustRefXmlResponse(apiresponse);

                            }

                        }
                        else
                        {
                            apiresponse.StatusCode = "29";
                            apiresponse.StatusDescription = "Invalid Merchant Code";
                            xmlResponse = GetValidateCustRefXmlResponse(apiresponse);
                        }
                    }
                    else
                    {
                        apiresponse.StatusCode = "29";
                        apiresponse.StatusDescription = "Merchant Customer Not Referenced";
                        xmlResponse = GetValidateCustRefXmlResponse(apiresponse);
                    }
                   

                }
                else
                {
                    apiresponse.StatusCode = "30";
                    apiresponse.StatusDescription = "Invalid Digital Signature";
                    xmlResponse = GetValidateCustRefXmlResponse(apiresponse);
                }

            }
            else
            {
                apiresponse.StatusCode = "100";
                apiresponse.StatusDescription = "Invalid Vendor Credentials";
                xmlResponse = GetValidateCustRefXmlResponse(apiresponse);
            }
            return xmlResponse;
        }


        public string GetMerchantPaymentDetailsApi(Request vmRequest)
        {
            ListenerResponse resp = new ListenerResponse();
            string xmlResponse = "";
            resp = GetVendorCredentials(vmRequest.Username, vmRequest.Password);
            if (resp.StatusCode.Equals("0") && resp.StatusDescription.Equals("ValidVendor"))
            {
                if (IsValidDigitalSignature(vmRequest))
                {
                    mapiRequest.ReferenceId = vmRequest.MerchantCode;
                    mapiRequest.MerchantId = vmRequest.MerchantCode;
                    mapiRequest.RequestAgent = vmRequest.CustomerTel;
                    mapiRequest.ApiUsername = apiUsername;
                    mapiRequest.ApiPassword = apiPassword;
                    mapiRequest.Channel = vmRequest.Channel;
                    apiResp = mapi.GetMerchantDetails(mapiRequest);
                    if (apiResp.Status.Equals("0"))
                    {

                        mapiRequest.Amount = vmRequest.Amount;
                        mapiRequest.ReferenceId = apiResp.Customer.AccountNumber;
                        MerchantChargeResponse chargeResp = mapi.GetCharges(mapiRequest);
                        if (chargeResp.StatusCode.Equals("0"))
                        {
                            mtran.DigitalSignature = "1234";
                            mtran.ApiUsername = apiUsername;
                            mtran.ApiPassword = apiPassword;
                            mtran.Channel = vmRequest.Channel;
                            mtran.CustTel = vmRequest.CustomerTel;//bll.DecryptPhone(phone);
                            mtran.AccountTo = apiResp.Customer.AccountNumber;
                            mtran.AccountFrom = vmRequest.CustomerTel;
                            mtran.Amount = vmRequest.Amount;
                            mtran.TranType = "TOPUP";
                            mtran.PaymentType = "MTN MOBILE MONEY";
                            mtran.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy");
                            mtran.TranId = vmRequest.RequestId;
                            mtran.MerchantId = vmRequest.MerchantCode;
                            mtran.CustRef = vmRequest.CustomerRef;
                            mtran.CustName = vmRequest.MerchantCode;
                            mtran.CreatedBy = "108633";
                            mtran.CustomerType = "PUBLIC";
                            mtran.Utility = "";
                            mtran.Area = vmRequest.MerchantCode;
                            mtran.Field1 = "";
                            mtran.Narration = vmRequest.Narration;
                            mtran.Charge = chargeResp.CalculatedChargeAmount;
                            payResp = mapi.MakePegPayTopUp(mtran);
                            payResp.RequestId = vmRequest.RequestId;
                            xmlResponse = GetMerchantPaymentXmlResponse(payResp);
                        }
                        else
                        {
                            apiResp.Status = chargeResp.StatusCode;
                            apiResp.StatusDescription = chargeResp.StatusDesc;
                            xmlResponse = GetMerchantPaymentXmlResponse(apiResp);
                        }

                      
                    }
                    else
                    {
                        apiResp.Status = apiResp.Status;
                        apiResp.StatusDescription = apiResp.StatusDescription;
                        xmlResponse = GetMerchantPaymentXmlResponse(apiResp);
                    }
                   
                }
                else
                {
                    apiResp.Status = "30";
                    apiResp.StatusDescription = "Invalid Digital Signature";
                    xmlResponse = GetMerchantPaymentXmlResponse(apiResp);
                }

            }
            else
            {
                apiResp.Status = "100";
                apiResp.StatusDescription = "Invalid Vendor Credentials";
                xmlResponse = GetMerchantPaymentXmlResponse(apiResp);
            }


            return xmlResponse;
        }

        public string GetMerchantChargeDetailsApi(Request vmRequest)
        {
            ListenerResponse resp = new ListenerResponse();
            MerchantChargeResponse chargeResp = new MerchantChargeResponse();
            string xmlResponse = "";
            resp = GetVendorCredentials(vmRequest.Username, vmRequest.Password);
            if (resp.StatusCode.Equals("0") && resp.StatusDescription.Equals("ValidVendor"))
            {
                if (IsValidDigitalSignature(vmRequest))
                {
                    mapiRequest.ReferenceId = vmRequest.MerchantCode;
                    mapiRequest.MerchantId = vmRequest.MerchantCode;
                    mapiRequest.RequestAgent = "1";
                    mapiRequest.ApiUsername = apiUsername;
                    mapiRequest.ApiPassword = apiPassword;
                    mapiRequest.Channel = vmRequest.Channel;
                    apiResp = mapi.GetMerchantDetails(mapiRequest);
                    if (apiResp.Status.Equals("0"))
                    {

                        mapiRequest.Amount = vmRequest.Amount;
                        mapiRequest.ReferenceId = apiResp.Customer.AccountNumber;
                        chargeResp = mapi.GetCharges(mapiRequest);
                        if (chargeResp.StatusCode.Equals("0"))
                        {

                            xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
                        }
                        else
                        {
                            apiResp.Status = chargeResp.StatusCode;
                            apiResp.StatusDescription = chargeResp.StatusDesc;
                            xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
                        }

                    }
                    else
                    {
                        chargeResp.Status = apiResp.Status;
                        chargeResp.StatusDesc = apiResp.StatusDescription;
                        xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
                    }

                }
                else
                {
                    chargeResp.Status = "30";
                    chargeResp.StatusDesc = "Invalid Digital Signature";
                    xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
                }

            }
            else
            {
                chargeResp.Status = "100";
                chargeResp.StatusDesc = "Invalid Vendor Credentials";
                xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
            }
            
            return xmlResponse;
        }

        public ListenerResponse GetVendorCredentials(string vendorcode, string password)
        {
            ListenerResponse resp = new ListenerResponse();
            DataTable vendordetails = dbh.GetVendorDetails(vendorcode, HashPassword(password));
            if (vendordetails.Rows.Count>0)
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "ValidVendor";
            }
            else
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "InValidVendor VendorCredentials";
            }
            return resp;
        }

        public string HashPassword(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2", CultureInfo.CurrentCulture));
                return sb.ToString();
            }
        }


        public bool IsValidDigitalSignature(Request request)
        {
            bool valid = false;
            try
            {
                string text = request.Username + request.Password + request.MerchantCode + request.Channel + request.RequestId;
                string certPath = "C:\\PegPayCertificates1\\";
                certPath = certPath + "\\" + request.Username + "\\";
                string[] fileEntries = Directory.GetFiles(certPath);
                string filePath = "";
                if (fileEntries.Length == 1)
                {
                    filePath = fileEntries[0].ToString();
                    X509Certificate2 cert = new X509Certificate2(filePath);
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                    SHA1Managed sha1 = new SHA1Managed();
                    //UnicodeEncoding encoding = new UnicodeEncoding();
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(text);
                    byte[] hash = sha1.ComputeHash(data);
                    byte[] sig = Convert.FromBase64String(request.DigitalSignature);
                    valid = csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);

                    //return true;
                }
                else
                {
                    valid = false;
                }
            }
            catch(Exception ex)
            {
                valid = false;
            }
         
            return valid;
        }

        public string GetValidationXmlResponse(Response apiResp)
        {
            string xml = "";
            if (apiResp.Status.Equals("0"))
            {
               // string TransactionID = DateTime.Now.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Ticks + "");
                xml = "<? xml version = \"1.0\" encoding =\"UTF-8\" ?>" +
                        "<ValidateMerchantResponse>" +
                        "<MerchantName>" + apiResp.Customer.FullName + "</MerchantName>" +
                       "<MerchantCode>" + apiResp.Customer.MerchantId + "</MerchantCode >" +
                        "<StatusCode>" + apiResp.Status + "</StatusCode>" +
                        "<StatusDescription>" + apiResp.StatusDescription + "</StatusDescription >" +
                       "</ValidateMerchantResponse>";
            }
            else
            {
                xml = "<? xml version = \"1.0\" encoding =\"UTF-8\" ?>" +
                      "<ValidateMerchantResponse>" +
                      "<MerchantName></MerchantName>" +
                      "<MerchantType></MerchantType>" +
                      "<MerchantCode></MerchantCode >" +
                      "<StatusCode>" + apiResp.Status + "</StatusCode>" +
                      "<StatusDescription>" + apiResp.StatusDescription + "</StatusDescription >" +
                     "</ValidateMerchantResponse>";
            }
            return xml;
        }

        public string GetMerchantPaymentXmlResponse(Response apiResp)
        {
            string xml = "";
            if (apiResp.Status.Equals("0"))
            {
                // string TransactionID = DateTime.Now.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Ticks + "");
                xml = "<? xml version = \"1.0\" encoding =\"UTF-8\" ?>" +
                        "<MakeMerchantPaymentResponse>" +
                       "<TransactionID>" + apiResp.RequestId+ "</TransactionID >" +
                        "<StatusCode>" + apiResp.Status + "</StatusCode>" +
                        "<StatusDescription>" + apiResp.StatusDescription + "</StatusDescription >" +
                       "</MakeMerchantPaymentResponse>";
            }
            else
            {
                xml = "<? xml version = \"1.0\" encoding =\"UTF-8\" ?>" +
                          "<MakeMerchantPaymentResponse>" +
                         "<TransactionID></TransactionID>" +
                          "<StatusCode>" + apiResp.Status + "</StatusCode>" +
                          "<StatusDescription>" + apiResp.StatusDescription + "</StatusDescription >" +
                         "</MakeMerchantPaymentResponse>";
            }
            return xml;
        }

        public string GetMerchantChargeXmlResponse(MerchantChargeResponse apiCResp)
        {
            string xml = "";
            if (apiCResp.StatusCode.Equals("0"))
            {
                // string TransactionID = DateTime.Now.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Ticks + "");
                xml = "<? xml version = \"1.0\" encoding =\"UTF-8\" ?>" +
                        "<GetChargeResponse>" +
                       "<MerchantCode>" + apiCResp.MerchantId + "</MerchantCode>" +
                       "<Amount>" + apiCResp.OriginalTransactionAmount+ "</Amount>" +
                       "<Charge>" + apiCResp.CalculatedChargeAmount + "</Charge>" +
                       "<ChargeCode>" + apiCResp.ChargeCode + "</ChargeCode>" +
                        "<StatusCode>" + apiCResp.StatusCode + "</StatusCode>" +
                        "<StatusDescription>" + apiCResp.StatusDesc + "</StatusDescription>" +
                       "</GetChargeResponse>";
            }
            else
            {
                xml = "<? xml version = \"1.0\" encoding =\"UTF-8\" ?>" +
                        "<GetChargeResponse>" +
                       "<MerchantCode></MerchantCode>" +
                       "<Amount></Amount>" +
                       "<Charge></Charge>" +
                       "<ChargeCode></ChargeCode>" +
                        "<StatusCode>" + apiCResp.StatusCode + "</StatusCode>" +
                        "<StatusDescription>" + apiCResp.StatusDesc + "</StatusDescription>" +
                       "</GetChargeResponse>";
            }
                return xml;
        }

        public void LogRequestResponse(string request, string response, string requestid, string method, string vendorcode, string sourceip, string serverip)
        {
            dbh.LogRequestResponse(request,  response,  requestid, method, vendorcode,  sourceip, serverip);
        }

        public Request GetValidateMerchantRequestDetails(string request)
        {
            Request req = new Request();
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(request);
            XmlNodeList GetValidateMerchantList = XmlRequest.GetElementsByTagName("ValidateMerchant");

            //this should loop once
            foreach (XmlNode node in GetValidateMerchantList)
            {
                try
                {
                    req.Username = node.SelectSingleNode("Username").InnerText;
                    req.Password = node.SelectSingleNode("Password").InnerText;
                    req.MerchantCode = node.SelectSingleNode("MerchantCode").InnerText;
                    req.Channel = node.SelectSingleNode("Channel").InnerText;
                    req.RequestId = node.SelectSingleNode("RequestId").InnerText;
                    req.DigitalSignature = node.SelectSingleNode("DigitalSignature").InnerText;
                    req.StatusCode = "0";
                    req.StatusDescription = "SUCCESS";
                }
                catch (Exception e)
                {
                    req.MerchantCode = "";
                    req.StatusCode = "200";
                    req.StatusDescription = "FAILED TO PARSE XML REQUEST";
                    LogError("", "ProcessThread", "", "", req.StatusDescription, "");
                }
                break;
            }
            return req;
        }

        public Request GetMerchantPaymentDetails(string request)
        {
            Request req = new Request();
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(request);
            XmlNodeList GetMerchantPaymentList = XmlRequest.GetElementsByTagName("MakeMerchantPayment");

            //this should loop once
            foreach (XmlNode node in GetMerchantPaymentList)
            {
                try
                {
                    req.Username = node.SelectSingleNode("Username").InnerText;
                    req.Password = node.SelectSingleNode("Password").InnerText;
                    req.MerchantCode = node.SelectSingleNode("MerchantCode").InnerText;
                    req.Amount = node.SelectSingleNode("Amount").InnerText;
                    req.CustomerRef = node.SelectSingleNode("CustomerRef").InnerText;
                    req.CustomerTel = node.SelectSingleNode("CustomerTel").InnerText;
                    req.Narration = node.SelectSingleNode("Narration").InnerText;
                    req.Channel = node.SelectSingleNode("Channel").InnerText;
                    req.RequestId= node.SelectSingleNode("RequestId").InnerText;
                    req.DigitalSignature = node.SelectSingleNode("DigitalSignature").InnerText;
                    req.TransactionDate= node.SelectSingleNode("TransactionDate").InnerText;
                    req.StatusCode = "0";
                    req.StatusDescription = "SUCCESS";
                }
                catch (Exception e)
                {
                    req.MerchantCode = "";
                    req.StatusCode = "200";
                    req.StatusDescription = "FAILED TO PARSE XML REQUEST";
                    LogError("", "ProcessThread", "", "", req.StatusDescription, "");
                }
                break;
            }
            return req;
        }

        public Request GetMerchantChargeRequestDetails(string request)
        {
            Request req = new Request();
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(request);
            XmlNodeList GetMerchantChargeList = XmlRequest.GetElementsByTagName("GetMerchantCharge");

            //this should loop once
            foreach (XmlNode node in GetMerchantChargeList)
            {
                try
                {
                    req.Username = node.SelectSingleNode("Username").InnerText;
                    req.Password = node.SelectSingleNode("Password").InnerText;
                    req.MerchantCode = node.SelectSingleNode("MerchantCode").InnerText;
                    req.Amount = node.SelectSingleNode("Amount").InnerText;
                    req.Channel = node.SelectSingleNode("Channel").InnerText;
                    req.RequestId = node.SelectSingleNode("RequestId").InnerText;
                    req.DigitalSignature = node.SelectSingleNode("DigitalSignature").InnerText;
                    req.StatusCode = "0";
                    req.StatusDescription = "SUCCESS";
                }
                catch (Exception e)
                {
                    req.MerchantCode = "";
                    req.StatusCode = "200";
                    req.StatusDescription = "FAILED TO PARSE XML REQUEST";
                    LogError("", "ProcessThread", "", "", req.StatusDescription, "");
                }
                break;
            }
            return req;
        }

        public Request GetMerchantTranStatusDetails(string request)
        {
            Request req = new Request();
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(request);
            XmlNodeList GetMerchantChargeList = XmlRequest.GetElementsByTagName("GetTransactionStatus");

            //this should loop once
            foreach (XmlNode node in GetMerchantChargeList)
            {
                try
                {
                    req.Username = node.SelectSingleNode("Username").InnerText;
                    req.Password = node.SelectSingleNode("Password").InnerText;
                    req.MerchantCode = node.SelectSingleNode("MerchantCode").InnerText;
                    req.Channel = node.SelectSingleNode("Channel").InnerText;
                    req.DigitalSignature = node.SelectSingleNode("DigitalSignature").InnerText;
                    req.TransactionId = node.SelectSingleNode("TransactionId").InnerText;
                    req.RequestId = node.SelectSingleNode("RequestId").InnerText;
                    req.StatusCode = "0";
                    req.StatusDescription = "SUCCESS";
                }
                catch (Exception e)
                {
                    req.MerchantCode = "";
                    req.StatusCode = "200";
                    req.StatusDescription = "FAILED TO PARSE XML REQUEST";
                    LogError("", "ProcessThread", "", "", req.StatusDescription, "");
                }

            }
            return req;
        }

        public Request GetValidateCustRefDetails(string request)
        {
            Request req = new Request();
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(request);
            XmlNodeList GetValidateCustRefList = XmlRequest.GetElementsByTagName("ValidateCustomerRef");

            //this should loop once
            foreach (XmlNode node in GetValidateCustRefList)
            {
                try
                {
                    req.Username = node.SelectSingleNode("Username").InnerText;
                    req.Password = node.SelectSingleNode("Password").InnerText;
                    req.MerchantCode = node.SelectSingleNode("MerchantCode").InnerText;
                    req.Channel = node.SelectSingleNode("Channel").InnerText;
                    req.DigitalSignature = node.SelectSingleNode("DigitalSignature").InnerText;
                    req.CustomerRef = node.SelectSingleNode("CustomerRef").InnerText;
                    req.RequestId = node.SelectSingleNode("RequestId").InnerText;
                    req.StatusCode = "0";
                    req.StatusDescription = "SUCCESS";
                }
                catch (Exception e)
                {
                    req.MerchantCode = "";
                    req.StatusCode = "200";
                    req.StatusDescription = "FAILED TO PARSE XML REQUEST";
                    LogError("", "ProcessThread", "", "", req.StatusDescription, "");
                }

            }
            return req;
        }

        private string GetTranRequestType(string requestXml)
        {
            string requestType = "";
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(requestXml);

            XmlNodeList GetValidationMerchantRequest = XmlRequest.GetElementsByTagName("ValidateMerchant");
            XmlNodeList GetValidationMerchantCharge = XmlRequest.GetElementsByTagName("GetMerchantCharge");
            XmlNodeList GetValidationMerchantPayment = XmlRequest.GetElementsByTagName("MakeMerchantPayment");
            XmlNodeList GetMerchantTranStatus = XmlRequest.GetElementsByTagName("GetTransactionStatus");
            XmlNodeList GetValidateCustomerRef= XmlRequest.GetElementsByTagName("ValidateCustomerRef");

            ////XmlNodeList GetFinancialInfoList = XmlRequest.GetElementsByTagName("ns2:getfinancialresourceinformationrequest");
            //XmlNodeList QueryLastToken = XmlRequest.GetElementsByTagName("Request_Type");
            //XmlNodeList ConfirmPaymentList = XmlRequest.GetElementsByTagName("ns0:paymentrequest");

            if (GetValidationMerchantRequest.Count > 0)
            {
                requestType = "ValidateMerchant";
            }
            else if(GetValidationMerchantPayment.Count>0)
            {
                requestType = "MakeMerchantPayment";
            }
            else if (GetValidationMerchantCharge.Count > 0)
            {
                requestType = "GetMerchantCharge";
            }
           else if (GetMerchantTranStatus.Count > 0)
            {
                requestType = "GetTransactionStatus";
            }
            else if (GetValidateCustomerRef.Count > 0)
            {
                requestType = "ValidateCustomerRef";
            }
            else
            {
                requestType = "UNKNOWN";
            }
            return requestType;
        }


        public string GetMerchantTranStatusApi(Request vmRequest)
        {

            ListenerResponse resp = new ListenerResponse();
            MerchantChargeResponse chargeResp = new MerchantChargeResponse();

            string xmlResponse = "";
            mapiRequest.ReferenceId = vmRequest.RequestId;
            mapiRequest.MerchantId = vmRequest.MerchantCode;
            mapiRequest.RequestAgent = "1";
            mapiRequest.ApiUsername = apiUsername;
            mapiRequest.ApiPassword = apiPassword;
            mapiRequest.Channel = vmRequest.Channel;
            apiResp = mapi.GetMerchantDetails(mapiRequest);
            if (apiResp.Status.Equals("0"))
            {

                mapiRequest.Amount = vmRequest.Amount;
                mapiRequest.ReferenceId = apiResp.Customer.AccountNumber;
                chargeResp = mapi.GetCharges(mapiRequest);
                if (chargeResp.StatusCode.Equals("0"))
                {

                    xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
                }
                else
                {
                    apiResp.Status = chargeResp.StatusCode;
                    apiResp.StatusDescription = chargeResp.StatusDesc;
                    xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
                }

            }
            else
            {
                chargeResp.Status = apiResp.Status;
                chargeResp.StatusDesc = apiResp.StatusDescription;
                xmlResponse = GetMerchantChargeXmlResponse(chargeResp);
            }

            return xmlResponse;
        }

        public string GetMerchantTranStatusXmlResponse(MerchantTransaction apiTResp)
        {
            string xml = "";
            string status = apiTResp.Status.ToString();
            if (!string.IsNullOrEmpty(status))
            {
                // string TransactionID = DateTime.Now.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Ticks + "");
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<GetTransactionStatus>" +
                        "<TransactionId>" + apiTResp.TranId + "</TransactionId>" +
                        "<statusCode>" + apiTResp.Status + "</statusCode>" +
                        "<statusDescription>" + apiTResp.StatusDescription + "</statusDescription>" +
                        "</GetTransactionStatus>";
            }
            else
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<GetTransactionStatus>" +
                        "<TransactionId></TransactionId>" +
                        "<statusCode>" + apiTResp.Status + "</statusCode>" +
                        "<statusDescription>" + apiTResp.StatusDescription + "</statusDescription>" +
                        "</GetTransactionStatus>";
            }
            return xml;
        }

        public string GetValidateCustRefXmlResponse(ValidateCustomerRefResponse apiVCResp)
        {
            string xml = "";
            if (apiVCResp.StatusCode.Equals("00"))
            {
                // string TransactionID = DateTime.Now.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Ticks + "");
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<ValidateCustomerReferenceResponse>" +
                        "<MerchantCode>" + apiVCResp.MerchantCode+ "</MerchantCode>" +
                        "<MerchantName>" + apiVCResp.MerchantName + "</MerchantName>" +
                        "<CustomerName>" + apiVCResp.CustomerName + "</CustomerName>" +
                        "<OutstandingBalance>" + apiVCResp.OutstandingBalance + "</OutstandingBalance>" +
                        "<MinimumBalance>" + apiVCResp.MinimumBalance + "</MinimumBalance>" +
                        "<statusCode>" + apiVCResp.StatusCode + "</statusCode>" +
                        "<statusDescription>" + apiVCResp.StatusDescription + "</statusDescription>" +
                        "</ValidateCustomerReferenceResponse>";
            }
            else
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<ValidateCustomerReferenceResponse>" +
                        "<MerchantCode></MerchantCode>" +
                        "<MerchantName></MerchantName>" +
                        "<CustomerName></CustomerName>" +
                        "<OutstandingBalance></OutstandingBalance>" +
                        "<MinimumBalance></MinimumBalance>" +
                        "<statusCode>" + apiVCResp.StatusCode + "</statusCode>" +
                        "<statusDescription>" + apiVCResp.StatusDescription + "</statusDescription>" +
                        "</ValidateCustomerReferenceResponse>";
            }
            return xml;
        }

        public void LogError(string VendorCode, string Method, string MerchantCode, string ErrorType, string Message, string RequestId)
        {
            dbh.LogError(VendorCode, Method, MerchantCode, ErrorType, Message,  RequestId);
        }
        public bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }



        public string GetServerIpIpValue()
        {
            string ipaddress = null;
            try
            {
                object addresslist = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                foreach (IPAddress ip in (Dns.GetHostEntry(Dns.GetHostName()).AddressList))
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipaddress = ip.ToString();
                    }
                    else
                    {
                        ipaddress = ip.ToString();
                    }
                }

                if (String.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "localhostip";
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                string method = methodBase.DeclaringType.Name + "." + methodBase.Name;
                LogError("", method, "", "Exception", ex.Message, "");
            }

            return ipaddress;
        }


    }
}
