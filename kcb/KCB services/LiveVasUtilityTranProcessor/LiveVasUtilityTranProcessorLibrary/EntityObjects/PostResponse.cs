using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveVasUtilityTranProcessorLibrary.EntityObjects
{
    public class PostResponse
    {
        private string pegpayPostId, statusCode, statusDesc;

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
                return statusDesc;
            }
            set
            {
                statusDesc = value;
            }
        }

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
    }
}
