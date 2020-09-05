using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public interface IVendorUtilityAccess
    {
        bool isValidVendorUtilityMapping();
        bool isValidVendorTraficIpAccess();
        bool isActiveVendor(string vendorCode, DataTable vendorData);
        bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData);
    }
}
