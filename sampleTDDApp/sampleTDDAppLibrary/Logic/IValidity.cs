using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public interface IValidity
    {
        bool isActiveVendor(string vendorCode, DataTable vendorData);
        bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData);
        bool isSignatureValid(Transaction trans);
        bool isValidVendorUtilityMapping();
        bool isValidVendorTraficIpAccess();
        bool IsduplicateVendorRef(Transaction trans);
        bool IsValidReversalStatus(Transaction trans);
        
    }
}
