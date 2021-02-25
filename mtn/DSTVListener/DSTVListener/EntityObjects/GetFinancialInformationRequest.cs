using System;
using System.Collections.Generic;
using System.Text;

namespace DSTVListener.EntityObjects
{
    public class GetFinancialInformationRequest:Request
    {
        public string CustomerTel;
        public string FRIRequestType;
        public string SmartCardNumber;
        public string BoxOfficeAmount;
        public string UtilityCode;
        public string BouquetCode;

        public bool IsValidRequest()
        {
            StatusCode = "0";
            StatusDescription = "SUCCESS";
            return true;
        }
    }
}
