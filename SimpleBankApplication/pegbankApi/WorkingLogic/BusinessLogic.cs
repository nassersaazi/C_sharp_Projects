using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WorkingLogic.EntityObjects;

namespace WorkingLogic
{
    public class BusinessLogic
    {
        
        //public void LogRequestAndResponse(string Method, string requestData, DateTime requestTime, string responseData, DateTime responseTime, string CustomerReference, string ReferenceId)
        //{
        //    //PostTransactionResponse resp = new PostTransactionResponse();
        //    try
        //    {
        //        dh.LogRequestAndResponse(Method, requestData, requestTime, responseData, responseTime, CustomerReference, ReferenceId);
        //    }
        //    catch (Exception e)
        //    {
        //        dh.LogError("Exception " + e.Message, "STANBIC_VAS", DateTime.Now, ReferenceId);
        //    }
        //}
        public void Serialize(object dataToSerialize)
        {
            DatabaseHandler dh = new DatabaseHandler();
            using (StringWriter stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                string xml = stringwriter.ToString();
                dh.SaveRequestlog(xml, DateTime.Now);
            }
        }

        public int SaveClientToDatabase(PostTransactionRequest requestData)
        {
            DatabaseHandler dh = new DatabaseHandler();
            int result = 0;
            try
            {
                return dh.InsertIntoClientTable(requestData);

            }
            catch (Exception e)
            {
                dh.LogError(DateTime.Now, e.Message, "");
                return result;
            }
            
        }

        public string GenerateRandomNNumber()
        {

            var rnd = new Random(DateTime.Now.Millisecond);
            int ticks = rnd.Next(0, 3000000);
            return "TXN" + ticks;
        }
    }
}
