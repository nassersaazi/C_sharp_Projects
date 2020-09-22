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
        
        bool isSignatureValid(ITransaction trans);
       
        bool IsduplicateVendorRef(ITransaction trans);
        bool IsValidReversalStatus(ITransaction trans);
        
    }
}
