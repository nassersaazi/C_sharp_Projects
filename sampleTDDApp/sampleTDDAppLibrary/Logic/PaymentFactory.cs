using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public class PaymentFactory
    {
        public Payment initialisePayment(NWSCTransaction transaction)
        {
            try
            {
                LogRequest(transaction);
            }
            catch (Exception)
            {

                return null;
            }
            switch (transaction.UtilityCode)
            {
                case "NWSC":
                    return new NWSCPayment();
                default:
                    return null;
            }
            
            
        }
        public void LogRequest(Transaction transaction)
        {
            // Log transaction request
        }

    }
}
