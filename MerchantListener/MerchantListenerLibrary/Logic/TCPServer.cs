using MerchantListenerLibrary.EntityObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerchantListenerLibrary.Logic
{
    class TCPServer
    {

        private Thread Worker;
        log4net.ILog Log = log4net.LogManager.GetLogger(typeof(TCPServer));
        public void ListenAndProcess()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:8085/externalmerchant/");
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
                    //ProcessThread(context);
                }
                catch (Exception ex)
                {

                }
            }
        }

        //public void ProcessThread(object httpContext)
        //{
        //    BusinessLogic bl = new BusinessLogic();
        //    ListenerResponse resp = new ListenerResponse();
        //    HttpListenerContext context = (HttpListenerContext)httpContext;
        //    string xmlResponse = "";
        //    try
        //    {
        //        string SourceIp = "";
        //        string ServerIp = "";

                
        //        SourceIp = bl.GetRequestIp();
        //        ServerIp = bl.GetServerIpIpValue();
        //        string request = (new StreamReader(context.Request.InputStream).ReadToEnd());

        //        Console.WriteLine("------------Request Made---------");
        //        Console.WriteLine(request + " " + DateTime.Now.ToString());
        //        string startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");

        //        string StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
        //        Log.Info("----------------------------Request Made------------------------------\n" + StartTime + Environment.NewLine + request);

        //        ServicePointManager.ServerCertificateValidationCallback = bl.RemoteCertificateValidation;
        //        xmlResponse = bl.ProcessRequest(request, SourceIp, ServerIp);
        //        string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
        //        Log.Info(".........................Response to Request......................\n" + endTime + Environment.NewLine + xmlResponse + Environment.NewLine);
        //        Console.WriteLine(".........................Response to Request......................");
        //        Console.WriteLine(xmlResponse + " " + DateTime.Now.ToString());
                
                

        //    }
        //    catch (Exception ex)
        //    {
        //        resp.StatusCode = "200";
        //        resp.StatusDescription = ex.Message;
        //        bl.LogError("", "ProcessThread", "", "Exception", ex.Message, "");
                


        //    }

        //    byte[] buf = Encoding.ASCII.GetBytes(xmlResponse);
        //    context.Response.ContentLength64 = buf.Length;
        //    context.Response.OutputStream.Write(buf, 0, buf.Length);

        //}
    }
}
