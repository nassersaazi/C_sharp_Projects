using kcbTester.EntityObjects;
using System;
using System.Collections.Generic;
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

