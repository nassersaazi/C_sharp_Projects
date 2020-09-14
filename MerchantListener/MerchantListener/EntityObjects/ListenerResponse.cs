using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantListener.EntityObjects
{
    class ListenerResponse
    {

        private string statusCode, statusDesc,amount,charge,tranid,merchantCode,merchantName;

        public string Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string MerchantName
        {
            get { return merchantName; }
            set { value=merchantName; }
        }

        public string MerchantCode
        {
            get { return merchantCode; }
            set { value = merchantCode; }
        }

        public string Charge
        {
            get { return charge; }
            set { charge = value; }
        }

        public string TransId
        {
            get { return tranid; }
            set { tranid = value; }
        }


        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        public string StatusDescription
        {
            get { return statusDesc; }
            set { statusDesc = value; }
        }

     
    }
}
