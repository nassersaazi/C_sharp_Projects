using System;
using System.Collections.Generic;
using System.Text;

namespace DSTVListener.EntityObjects
{
    public class ConfirmPaymentResponse:Response
    {
        public string ThirdPartyAcctRef;
        public string Token;
    }
}
