using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public class PaymentFactory
    {
        
        public IPayment initialisePayment(string utilityCode)
        {
            
            switch (utilityCode)
            {
                case "NWSC":
                    return new NWSCPayment();
                default:
                    return null;
            }
            
            
        }
        
    }
}
