using sampleTDDAppLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sampleTDDApp.Logic
{
    public class Payment
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();


        public PostResponse MakePayment(NWSCTransaction trans)
        {
            return resp;
        }

    }
}
