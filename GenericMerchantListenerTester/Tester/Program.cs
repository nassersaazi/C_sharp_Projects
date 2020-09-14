using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Tester
{
    public class ValidateCustomerReferenceResponse
    {
        private string merchantName, merchantCode, customerName, oustandingBalance, minimumBalance, statusCode, statusDescription;

        private bool end, log;

        public string MerchantName
        {
            get
            {
                return merchantName;
            }
            set
            {
                merchantName = value;
            }
        }

        public string MerchantCode
        {
            get
            {
                return merchantCode;
            }
            set
            {
                merchantCode = value;
            }
        }

        public string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                customerName = value;
            }
        }
        public string OutstandingBalance
        {
            get
            {
                return oustandingBalance;
            }
            set
            {
                oustandingBalance = value;
            }
        }
        public string MinimumBalance
        {
            get
            {
                return minimumBalance;
            }
            set
            {
                minimumBalance = value;
            }
        }
        public string StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                statusCode = value;
            }


        }

        public string StatusDescription
        {
            get
            {
                return statusDescription;
            }
            set
            {
                statusDescription = value;
            }


        }
    }

    public static class XML
    {
        //public static string Serialize(Transaction value)
        //{
        //    if (value == null)
        //    {
        //        return string.Empty;
        //    }
        //    try
        //    {
        //        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        //        ns.Add("", "");
        //        //XmlSerializer s = new XmlSerializer(typeof(Transaction),"");
        //        //StringWriter myWriter = new StringWriter();
        //        //s.Serialize(myWriter, value, ns);

        //        //return myWriter.ToString();
        //        var xmlserializer = new XmlSerializer(typeof(Transaction), "");
        //        var stringWriter = new StringWriter();
        //        using (var writer = XmlWriter.Create(stringWriter))
        //        {
        //            xmlserializer.Serialize(writer, value, ns);
        //            return stringWriter.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred", ex);
        //    }
        //}

        public static ValidateCustomerReferenceResponse Deserialize(string value)
        {

            try
            {
                ValidateCustomerReferenceResponse result = new ValidateCustomerReferenceResponse();
                XmlSerializer serializer = new XmlSerializer(typeof(ValidateCustomerReferenceResponse));
                using (StringReader reader = new StringReader(value))
                {
                    result = (ValidateCustomerReferenceResponse)serializer.Deserialize(reader);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }

        }
    }
    public class TesttListener
    {
      

      

 



    }
    public class Program
    {
        static void Main(string[] args)
        {

            var sample = new TesttListener();
            string request = "";
            Console.WriteLine("1."+"Validate Merchant Customer");
            Console.WriteLine("2." + "ValidatCustomer Reference");
            Console.WriteLine("3." + "Get Merchant Charges");
            Console.WriteLine("4." + "Make Merchant Payment");
            Console.WriteLine("5." + "Get Transaction Status");
          

            request = Convert.ToString(Console.ReadLine());
            ProcessRequest pr = new ProcessRequest();
            pr.DetermineRequestType(request);
            Console.ReadLine();

        }

        public void ProcessTransaction(string transaction)
        {
            string url = "https://192.168.120.44:3999/externalmerchant/";

            try
            {
                // PegPayGateway gatewayApi = new PegPayGateway();


                //Console.WriteLine("Posted To Gateway at: " + DateTime.Now.ToString() );
                ///post to listener

                string postData = transaction;//"<?xml version=\"1.0\" encoding=\"utf-16\"?><Transaction><CustomerRef>" + tran.CustomerRef + "</CustomerRef><CustomerName>" + tran.CustomerName + "</CustomerName><Title>" + tran.Title + "</Title><PhoneNumber>" + tran.PhoneNumber + "</PhoneNumber><UserName>" + tran.UserName + "</UserName><Password>" + tran.Password + "</Password><DigitalSignature>" + tran.DigitalSignature + "</DigitalSignature><PaymentDate>" + tran.PaymentDate + "</PaymentDate><TransactionAmount>" + tran.TransactionAmount + "</TransactionAmount><TransactionType>" + tran.TransactionType + "</TransactionType><TransactionCategory>" + tran.TransactionCategory + "</TransactionCategory><TransactionRef>" + tran.TransactionRef + "</TransactionRef><Currency>" + tran.Currency + "</Currency><MerchantId>" + tran.MerchantId + "</MerchantId><Narration>" + tran.Narration + "</Narration><Teller>" + tran.Teller + "</Teller></Transaction>";//XML.Serialize(tran);

                var request = (HttpWebRequest)WebRequest.Create(url);

                var data = Encoding.ASCII.GetBytes(postData);

                X509Certificate Cert = GetCertificate();
                //   X509Certificate2 cert = new X509Certificate2(certPath, certPassword, X509KeyStorageFlags.MachineKeySet);


                request.ContentLength = data.Length;
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;

                request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "text/xml";

                request.ClientCertificates.Add(Cert);

                //request.Method = "POST";
                //request.ContentType = "text/xml";
                //request.ContentLength = data.Length;
                //System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;

                using (var syncStream = Stream.Synchronized(request.GetRequestStream()))
                {
                    syncStream.Write(data, 0, data.Length);
                    syncStream.Close();
                    syncStream.Dispose();
                }

                request.Timeout = 40000;
                Console.WriteLine("Posted To Gateway  at: " + DateTime.Now.ToString());
                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //var gatewayResp = XML.Deserialize(responseString);

                Console.WriteLine("Response From Gateway at: " + DateTime.Now.ToString() + " \n" + responseString);

                Console.ReadLine();


            }
            catch (Exception ex)
            {

            }
        }

        static X509Certificate2 GetCertificate()
        {
            try
            {
                string text = "‎‎00 b6 36 1c 3d 47 84 a8 4e";

                var rgx = new Regex("[^a-fA-F0-9]");
                var serial = rgx.Replace(text, string.Empty).ToUpper();
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, false);
                store.Close();
                return certs[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
