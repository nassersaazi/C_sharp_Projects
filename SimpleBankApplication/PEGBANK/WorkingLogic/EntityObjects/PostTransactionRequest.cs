using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingLogic.EntityObjects
{
 

    public class PostTransactionRequest
    {


        private PostTransactionRequestPayLoad payload;



        public PostTransactionRequestPayLoad PayLoad
        {
            get { return payload; }
            set { payload = value; }
        }

        public PostTransactionRequest()
        {
            this.payload = new PostTransactionRequestPayLoad();
        }

        public class PostTransactionRequestPayLoad
        {

            private string accountNumber;
            private string name;
            private string email;
            private string accountBalance;


            public string AccountNumber
            {
                get { return accountNumber; }
                set { accountNumber = value; }
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }



            public string Email
            {
                get { return email; }
                set { email = value; }
            }


            public string AccountBalance
            {
                get { return accountBalance; }
                set { accountBalance = value; }
            }





            public PostTransactionRequestPayLoad()
            {
                this.accountNumber = "";
                this.name = "";
                this.email = "";
                this.accountBalance = "";

            }
        }


    }
}
