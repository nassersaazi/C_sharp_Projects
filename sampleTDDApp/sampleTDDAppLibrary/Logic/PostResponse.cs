using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public class PostResponse : Response
    {
        private string pegpayPostId;
        private string token;
        private string units;

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
    }
}
