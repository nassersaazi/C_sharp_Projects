using MerchantListener.EntityObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerchantListener.Logic
{
    class TCPServer
    {

        private Thread Worker;
        log4net.ILog Log = log4net.LogManager.GetLogger(typeof(TCPServer));
        public void ListenAndProcess()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("https://127.0.0.1:3999/externalmerchant/");
            listener.Start();
            while (true)
            {
                try
                {
                    string timeStamp = DateTime.Now.ToString("YYYYMMddHHss");
                    Console.WriteLine("Listening for Http Request.........");
                    HttpListenerContext context = listener.GetContext();
                    string _custIP = context.Request.RemoteEndPoint.ToString();
                    // Worker = new Thread(new ParameterizedThreadStart(ProcessThread));
                    // Worker.Start(context);
                    ProcessThread(context);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void ProcessThread(object httpContext)
        {
            BusinessLogic bl = new BusinessLogic();
            ListenerResponse resp = new ListenerResponse();
            try
            {
                string SourceIp = "";
                string ServerIp = "";

                HttpListenerContext context = (HttpListenerContext)httpContext;
                SourceIp = bl.GetRequestIp();
                ServerIp = bl.GetServerIpIpValue();
                string request = (new StreamReader(context.Request.InputStream).ReadToEnd());

                Console.WriteLine("------------Request Made---------");
                Console.WriteLine(request + " " + DateTime.Now.ToString());
                string startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");

                string StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                Log.Info("----------------------------Request Made------------------------------\n" + StartTime + Environment.NewLine + request);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = bl.RemoteCertificateValidation;
               string  xmlResponse = bl.ProcessRequest(request, SourceIp,ServerIp);
                string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                Log.Info(".........................Response to Request......................\n" + endTime + Environment.NewLine + xmlResponse + Environment.NewLine);
                Console.WriteLine(".........................Response to Request......................");
                Console.WriteLine(xmlResponse + " " + DateTime.Now.ToString());
                byte[] buf = Encoding.ASCII.GetBytes(xmlResponse);
                context.Response.ContentLength64 = buf.Length;
                context.Response.OutputStream.Write(buf, 0, buf.Length);
             
            }
            catch (Exception ex)
            {
                throw ex;
                resp.StatusCode = "200";
                resp.StatusDescription = ex.Message;
                bl.LogError("", "ProcessThread","","Exception",ex.Message,"");
            }

           
        }
    }
}
