using MerchantListenerLibrary.EntityObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerchantListenerLibrary.Logic
{
    public class TCPServer
    {

        private Thread Worker;
        log4net.ILog Log = log4net.LogManager.GetLogger(typeof(TCPServer));
        BusinessLogic bl = new BusinessLogic();
        public void ListenAndProcess()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:8085/externalmerchant/");
            listener.Start();
            while (true)
            {
                // var worker1 = new Thread(() => DoWork(listener));
                //var worker2 = new Thread(() => DoWork(listener));
                //var worker3 = new Thread(() => DoWork(listener));
                //worker1.Start();
                //worker2.Start();
                //worker3.Start();
                DoWork(listener);


            }
        }

        public void DoWork(HttpListener listener)
        {
            string timeStamp = DateTime.Now.ToString("YYYYMMddHHss");
            Console.WriteLine("Listening for Http Request.........");

            try
            {
                HttpListenerContext context = listener.GetContext();
                string _custIP = context.Request.RemoteEndPoint.ToString();
                //Worker = new Thread(new ParameterizedThreadStart(ProcessThread));
                //Worker.Start();
                ProcessThread(context);
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                string method = methodBase.DeclaringType.Name + "." + methodBase.Name;
                bl.LogError("", method, "", "Exception", ex.Message, "");

            }
        }

        public void ProcessThread(object httpContext)
        {
            BusinessLogic bl = new BusinessLogic();
            ListenerResponse resp = new ListenerResponse();
            string xmlResponse = "";
            byte[] buf;
            string SourceIp = "";
            string ServerIp = "";
            HttpListenerContext context = (HttpListenerContext)httpContext;

            try
            {

                SourceIp = bl.GetRequestIp();
                ServerIp = bl.GetServerIpIpValue();
                string request = (new StreamReader(context.Request.InputStream).ReadToEnd());

                Console.WriteLine("------------Request Made---------");
                Console.WriteLine(request + " " + DateTime.Now.ToString());
                string startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");

                string StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                Log.Info("----------------------------Request Made------------------------------\n" + StartTime + Environment.NewLine + request);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = bl.RemoteCertificateValidation;
                xmlResponse = bl.ProcessRequest(request, SourceIp, ServerIp);
                string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                Log.Info(".........................Response to Request......................\n" + endTime + Environment.NewLine + xmlResponse + Environment.NewLine);
                Console.WriteLine(".........................Response to Request......................");
                Console.WriteLine(xmlResponse + " " + DateTime.Now.ToString());
                buf = Encoding.ASCII.GetBytes(xmlResponse);
                context.Response.ContentLength64 = buf.Length;
                context.Response.OutputStream.Write(buf, 0, buf.Length);
                Console.WriteLine(".........................Response to Request......................");
                Console.WriteLine(xmlResponse + " " + DateTime.Now.ToString());

            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                string method = methodBase.DeclaringType.Name + "." + methodBase.Name;
                bl.LogError("", method, "", "Exception", ex.Message, "");
                string errorResponse = "The request could not be processed at this time.Please try again later...";
                buf = Encoding.ASCII.GetBytes(errorResponse);
                context.Response.ContentLength64 = buf.Length;
                context.Response.OutputStream.Write(buf, 0, buf.Length);
                Console.WriteLine(".........................Response to Request......................");
                Console.WriteLine(errorResponse + " " + DateTime.Now.ToString());


            }



        }
    }
}
