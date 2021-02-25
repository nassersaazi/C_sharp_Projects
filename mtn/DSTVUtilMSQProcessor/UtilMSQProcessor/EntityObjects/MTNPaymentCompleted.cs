using System;
using System.Collections.Generic;
using System.Text;

namespace UtilMSQProcessor.EntityObjects
{
    public class MTNPaymentCompletedResponse
    {
        public string successfullyNotified;
        public string Reason;
        public string xmlResponse;
        public bool isFailureResponse;

        public MTNPaymentCompletedResponse()
        {
            isFailureResponse = false;
            successfullyNotified = "0";
            Reason = "";
            xmlResponse = "";
        }

        public bool HasBeenSuccessfullAtMtn()
        {
            if (successfullyNotified.Equals("1") && !isFailureResponse)
            {
                return true;
            }
            return false;
        }
    }
}
