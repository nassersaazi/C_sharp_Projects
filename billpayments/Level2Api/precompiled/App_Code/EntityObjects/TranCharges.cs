using System;
using System.Collections.Generic;
using System.Text;

namespace Tester
{
    public class TranCharges
    {
        string meterNumber, tranId, chargeName, charge, balance, token, accountNumber;

        public string Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public string AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }

        public string MeterNumber
        {
            get { return meterNumber; }
            set { meterNumber = value; }
        }

        public string TranId
        {
            get { return tranId; }
            set { tranId = value; }
        }

        public string ChargeName
        {
            get { return chargeName; }
            set { chargeName = value; }
        }

        public string Charge
        {
            get { return charge; }
            set { charge = value; }
        }

        public string Token
        {
            get { return token; }
            set { token = value; }
        }

    }
}
