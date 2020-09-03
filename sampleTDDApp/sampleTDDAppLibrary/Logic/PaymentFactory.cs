using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public class PaymentFactory
    {
        DatabaseHandler dp = new DatabaseHandler();

        public Payment initialisePayment(NWSCTransaction transaction)
        {
            
            switch (transaction.UtilityCode)
            {
                case "NWSC":
                    try
                    {
                        dp.SaveRequestlog(transaction.VendorCode, "NWSC", "POSTING", transaction.CustRef, transaction.Password);
                    }
                    catch (Exception)
                    {

                        return null;
                    }
                    return new NWSCPayment();
                default:
                    dp.SaveRequestlog(transaction.VendorCode, transaction.UtilityCode, "POSTING", transaction.CustRef, transaction.Password);
                    return null;
            }
            
            
        }
        
    }
}
