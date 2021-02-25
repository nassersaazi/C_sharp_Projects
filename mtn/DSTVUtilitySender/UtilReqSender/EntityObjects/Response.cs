using System;
using System.Collections.Generic;
using System.Text;

namespace UtilReqSender.EntityObjects
{
    public class Response
    {
        private string statusCode, statusDescription;

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
}
