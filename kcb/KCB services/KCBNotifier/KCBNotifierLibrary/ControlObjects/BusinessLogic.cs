using KCBNotifierLibrary.EntityObjects;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace KCBNotifierLibrary.ControlObjects
{
    public class BusinessLogic
    {
        internal void NotifyProcessedTransaction(Transaction tran)
        {

            Console.WriteLine("************************************************************");
            Console.WriteLine("Notifying transaction with reference Id : " + tran.VendorTranId + " to Bank");
            //TODO: Call KCB API
            //TODO: Pass necessary parameters
            //TODO: Response from KCB api?success,Set NotifiedVendor to 1 in Vas:failure ,setNotifiedVendor to 0

            // ... when at Pegasus, use this url
            string flex_request_xml_url = "http://172.31.121.17:7072/BillerEngineNew/BillerEngineWebService";
            string statusCode = tran.StatusCode;
            string statusDescription = tran.StatusDescription;
            string reference = tran.VendorTranId;


            string flex_soap_action = "";
            string flex_xml_rqst_message = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:web='http://webserviceserver.billerengine.kcb/'>
                                                   <soapenv:Header/>
                                                   <soapenv:Body>
                                                      <web:KCBBillerTransactionCallBack>
                                                         <!--Optional:-->
                                                         <UserName>pegususUser</UserName>
                                                         <!--Optional:-->
                                                         <Password>8e8189beeb9919c9e50ced69affb8e92e0deb89acd5bb5a9f4418e52c3b33eaac9b6c976af8cc12df77083e8b55ddd4ca91f862e388a481e23f613ea4abac3ae</Password>
                                                         <!--Optional:-->
                                                         <ServiceID>28</ServiceID>
                                                         <!--Optional:-->
                                                         <ServiceName>TRANSACTIONCALLBACK</ServiceName>
                                                         <!--Optional:-->" +
                                                         "<TransactionReference>" + reference + "</TransactionReference>\n\t" +
                                                         "<!--Optional:-->" +
                                                         "<TransactionStatus>" + statusCode + "</TransactionStatus>\n\t" +
                                                         "<!--Optional:-->" +
                                                         "<TransactionStatusDescription>" + statusDescription + "</TransactionStatusDescription>\n\t" +
                                                         @"<!--Optional:-->
                                                         <ReversalStatus>0</ReversalStatus>
                                                         <!--Optional:-->
                                                         <SessionId>23qaqeq28743</SessionId>
                                                         <!--Optional:-->
                                                         <Payload>
                                                            <!--You may enter ANY elements at this point-->
                                                         </Payload>
                                                      </web:KCBBillerTransactionCallBack>
                                                   </soapenv:Body>
                                                </soapenv:Envelope>";

            string flex_resp = SendRequest(flex_request_xml_url, flex_soap_action, flex_xml_rqst_message);

            if (String.IsNullOrEmpty(flex_resp))
            {
                Console.WriteLine("Empty response from KCB!!!");
            }
            else
            {
                //Console.WriteLine("Successfully notified KCB!!! for transaction with reference " + tran.VendorTranId);
                HandleUtilityResponse(flex_resp, tran);
            }
            //Console.WriteLine(PrintXML(flex_resp));
            //Console.ReadLine();

            // Thread.Sleep(new TimeSpan(0, 0, 5));
            Console.WriteLine("************************DONE!!!************************************");





        }


        #region ... 990: SendRequest
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

        #region ... 991 Request Headers
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


        #region ... 993: print XML
        public static string PrintXML(string xml)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException)
            {
                // Handle the exception
            }

            mStream.Close();
            writer.Close();

            return result;
        }
        #endregion

        internal void HandleUtilityResponse(string utilityResp, Transaction trans)
        {


            //log the response from the utility first
            LogUtilityResponse(utilityResp, trans);

            //depending on the response lets see how to handle
            DatabaseHandler dh = new DatabaseHandler();

         

            //mark transaction as successfull
            MarkTransactionAsNotified(trans, "SUCCESS");
            
        }

        private void MarkTransactionAsNotified(Transaction tran, string v)
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                Console.WriteLine("Tran :" + tran.VendorTranId + " Successfully notified to KCB");

                dh.UpdateTransactionStatus(tran.VendorTranId);
            }
            catch (Exception ee)
            {

                dh.LogError("MarkTransactionAsNotified: " + ee.Message, "KCB_VAS", DateTime.Now, tran.VendorTranId);
                throw ee;
            }
        }
        
        private void LogUtilityResponse(string response, Transaction tran)
        {
            DatabaseHandler dh = new DatabaseHandler();
            //string OtherData = "UtilityCode = " + tran.UtilityCompany + ", UtilityRef = " + response.PegPayPostId + ", StatusCode = " + response.StatusCode + " StatusDesc = " + response.StatusDescription;
            string OtherData = response;

            dh.InsertIntoUtilityResponseLogs(tran.VendorCode, tran.VendorTranId, "", "", OtherData);
        }


    }
}
