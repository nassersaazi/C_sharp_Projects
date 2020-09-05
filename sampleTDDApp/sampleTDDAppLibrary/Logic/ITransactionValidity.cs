using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public interface ITransactionValidity
    {
        
        bool isSignatureValid(Transaction trans);
       
        bool IsduplicateVendorRef(Transaction trans);
        bool IsValidReversalStatus(Transaction trans);
        
    }
}
