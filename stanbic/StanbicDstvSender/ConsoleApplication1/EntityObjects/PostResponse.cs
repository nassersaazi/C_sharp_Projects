using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1.EntityObjects
{
    public class PostResponse : Response
    {
        private string pegpayPostId;

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
