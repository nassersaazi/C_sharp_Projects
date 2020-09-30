using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace sampleTDDAppLibrary.Logic
{
    public class PostResponse : Response
    {
        private string pegpayPostId;
        private string token;
        private string units;
        DatabaseHandler dp = new DatabaseHandler();

        public string PegPayPostId
        {
            get
            {
                return pegpayPostId;
            }
            set
            {
                pegpayPostId = value;
            }
        }
        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }
        public string Units
        {
            get
            {
                return units;
            }
            set
            {
                units = value;
            }
        }

        public void HandleResponse(ITransaction trans, PostResponse resp, string status, string statusDescription)
        {

            if (status == "0")
            {
                resp.PegPayPostId = dp.PostTransaction((NWSCTransaction)trans, "NWSC");
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.PegPayPostId = "";
                resp.StatusCode = status;
                resp.StatusDescription = string.IsNullOrEmpty(statusDescription) ? dp.GetStatusDescr(resp.StatusCode) : statusDescription;
            }
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
    }
}
