using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilReqSender.ControlObjects
{
    public class PaymentResponse
    {
        string statusCode, statusDescription, transactionId, requestId;

        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        public string StatusDescription
        {
            get { return statusDescription; }
            set { statusDescription = value; }
        }

        public string TransactionId
        {
            get { return transactionId; }
            set { transactionId = value; }
        }

        public string RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }
    }
    }
