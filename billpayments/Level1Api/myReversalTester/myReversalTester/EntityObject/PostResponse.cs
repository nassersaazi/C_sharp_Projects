using System;
using System.Collections.Generic;
using System.Text;

namespace myReversalTester.EntityObject
{
    public class PostResponse : Response
    {
        private string pegpayPostId;
        private string noOfUnits;

        public string NoOfUnits
        {
            get
            {
                return noOfUnits;
            }
            set
            {
                noOfUnits = value;
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
