using System;
using System.Collections.Generic;
using System.Text;
using UtilReqSender.ControlObjects;

namespace UtilReqSender.EntityObjects
{
    public class BouquetDetails
    {
        public string BouquetCode;
        public string BouquetName;
        public string BouquetPrice;
        public string BouquetDescription;
        public string PayTvCode;
        public string StatusCode;
        public string StatusDescription;
        public int invoicePeriod;
        public BouquetDetails()
        {
        }

        public void UpdateBouquetPrice()
        {
            DatabaseHandler dh = new DatabaseHandler();
            dh.UpdateBouquetPrice(this);
        }
    }
}
