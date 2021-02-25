using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTV206ProcessorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Pick txns from mobile money received where sendToVendor = 1 and Status is SUCCESS
            // Insert the txns in bill payments with Status INSERTED
            // If insertion is successful, update sentToVendor = 1 in mobile money and PegpayTranId to the pegpayId

            // Follow startimes flow to bill payments

            //Test connection to VSC
        }
    }
}
