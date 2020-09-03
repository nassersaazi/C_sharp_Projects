using sampleTDDApp.Logic;
using System;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Security.Cryptography;
using sampleTDDAppLibrary.Logic;

namespace sampleTDDApp
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var p = new Program();
            var tran = p.GetTransaction();
            p.MakePayment(tran);
            
        }

        public NWSCTransaction GetTransaction()
        {
            NWSCTransaction transaction = new NWSCTransaction();
            transaction.CustRef = new Random().Next(1, int.MaxValue).ToString();
            transaction.CustName = "KESI INVESTMENTS LTD";
            transaction.CustomerType = "2";
            transaction.Area = "Kampala";
            transaction.utilityCompany = "NWSC";
            transaction.PaymentDate = "31/08/2020";
            transaction.TransactionAmount = "96158";
            transaction.TransactionType = "CASH";
            transaction.VendorCode = "STANBIC_VAS";
            transaction.Password = "53P48KU262";
            transaction.CustomerTel = "256779248579";
            transaction.Reversal = "0";
            transaction.Teller = "213487670";
            transaction.Offline = "0";
            transaction.DigitalSignature = "gHSIBys0MrRGmx78/WEl5CMnugeQUhBMXdYrZM2A8Frw+I64L38MhIuFBlDDQzTDHZ6XYOE7t/vIGEo55enDIL5DVHLU1ld5UZgH4GvktjSiaYxE5LzhIqhEfalQ/gowONpNMP1/1pG8wosb5p0Uve4i5QSHL+gOUx4969eTx78ISR+W0p/6bTjItjXwodtjejdzM0VlM0u4lPkFiOYeTq0zqCsRlLz32fFwj+dIvk/5UpJ6Ot2th41SVZyn9tqRT2oMCo4uqImYXjegzJnzmknr/5y5N7rqAhnX6Xgc9E39l+pjGa2FEIDCghDCCMmzfDhJE4xSXt69E37Ou/j0QQ==";
            transaction.Narration = "CUSTOMER NAME-SHAHZAD KAMALUDDIN UKANI:CUSTOMER ID-SHAHZAD85CONSUMER CODE-21287670:REFERENCE ID-18529084:VAS REQUEST AMOUNTUGX|96158.0";
            transaction.VendorTransactionRef = new Random().Next(1, int.MaxValue).ToString();
            transaction.UtilityCode = "NWSC";
            return transaction;
        }

        public void MakePayment(NWSCTransaction transaction)
        {
            var paymentFactory = new PaymentFactory();

            try
            {
                var payment = paymentFactory.initialisePayment(transaction);
                var response = payment.pay(transaction);

                Console.WriteLine(payment.Serialize(response));
            }
            catch (Exception)
            {

                Console.WriteLine($" Vendor {transaction.UtilityCode} does not exist");
            }
        }
    }
}
