using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTV206ProcessorLibrary
{
    public class Class1
    {

        // Thread 1 
        -Picks  DSTV transactions from ReceivedTransaction table in  mobile money db where status is SUCCESS and SentToVendor is 0
        -Calls Level 1 of bill payments to insert the transactions in ReceivedTransactions table in bill payments with status of INSERTED
        -If Insertion Successful
        // Thread 2

    }
}
