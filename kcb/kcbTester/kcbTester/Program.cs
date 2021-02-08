using kcbTester.ControlObjects;
using kcbTester.EntityObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kcbTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BusinessLogic bs = new BusinessLogic();
            Transaction tran = new Transaction();
            tran.Username = "KCB";
            tran.Password = "63T25KG001";
            tran.Area = "MSPTA1";
            tran.ServiceCode = "69";
            tran.CustomerRef = "05-01778";
           bs.ValidateCustomer(tran);

            //PhoneValidator pv = new PhoneValidator();
            //var check = pv.PhoneNumbersOk("256774784672");
        }
    }
}
