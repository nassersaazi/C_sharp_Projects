using System;
using System.Data.SqlClient;
using sampleTDDAppLibrary.Logic;

namespace sampleTDDApp
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var p = new Program();
            var tran = p.GetTransaction();
            var tranDSTV = p.GetDSTVTransaction();
            p.MakePayment(tran);
            //p.MakeDSTVPayment(tranDSTV);
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
            transaction.DigitalSignature = "gHSIBys0MrRGmx78/WEl5CMnugeQUhBMXdYrZM2A8Frw+I64L38MhIuFBlDDQzTDHZ6XYOE7t/vIGEo55enDIL5DVHLU1ld5UZgH4GvktjSiaYxE5LzhIqhEfalQ/gow"+
                "ONpNMP1/1pG8wosb5p0Uve4i5QSHL+gOUx4969eTx78ISR+W0p/6bTjItjXwodtjejdzM0VlM0u4lPkFiOYeTq0zqCsRlLz32fFwj+dIvk/5UpJ6Ot2th41SVZyn9tqRT2oMCo4uqImYXjegzJnzmknr/5y5N7"+
                "rqAhnX6Xgc9E39l+pjGa2FEIDCghDCCMmzfDhJE4xSXt69E37Ou/j0QQ==";
            transaction.Narration = "CUSTOMER NAME-SHAHZAD KAMALUDDIN UKANI:CUSTOMER ID-SHAHZAD85CONSUMER CODE-21287670:REFERENCE ID-18529084:VAS REQUEST AMOUNTUGX|96158.0";
            transaction.VendorTransactionRef = new Random().Next(1, int.MaxValue).ToString();
            transaction.UtilityCode = "NWSC";
            return transaction;
        }

        public DSTVTransaction GetDSTVTransaction()
        {
            DSTVTransaction transaction = new DSTVTransaction();
            transaction.CustRef = new Random().Next(1, int.MaxValue).ToString();
            transaction.CustName = "KONGAI VERONICAKATUSIIME";
            transaction.CustomerType = "2";
            transaction.Area = "TOP_UP";
            transaction.PaymentDate = "31/07/2020";
            transaction.TransactionAmount = "129000";
            transaction.TransactionType = "CASH";
            transaction.VendorCode = "STANBIC_VAS";
            transaction.Password = "53P48KU262";
            transaction.CustomerTel = "256772212675";
            transaction.Reversal = "0";
            transaction.Teller = "10533030358";
            transaction.Offline = "0";
            transaction.DigitalSignature = "cihstY9+yp0rPEF6H0kmX+/wWiCpmSw3vNySiDwkDCsnxG+b8gneSkGot0Shdz4uamXJbaYhIJHUGDUf9m1ynW7AAOk68ZR2jrDRpOoSIVjdOvarbyxn+eoA7/ZAxMtJ0LbNJGxGZJNtky+gR+3IbG3/A6E4Fvc6r59ZKUqIhgp5i0EqiJ6FJVw5jlGm5lDJQKqvlC5hldVpT4Rv7TTQ1WKWKi84/J5yDtYH95UWViK25cMaVHJSuo6TzWnzFVKepYVBbyxhlHjNV4nNU0SXDPLXeTv9vpaGkS4tZKNXDyizZVjFVOyID+EG3VFLIK7wGPBnomYwsqoWoytCtUFSHw==";
            transaction.Narration = "CUSTOMER NAME-KONGAI VERONICA KATUSIIME:CUSTOMER ID-KONGAVERONICACONSUMER CODE-10533030358:REFERENCE ID-17753995:VAS REQUEST AMOUNTUGX|129000.0";
            transaction.VendorTransactionRef = new Random().Next(1, int.MaxValue).ToString();
            transaction.UtilityCode = "DSTV";
            return transaction;
        }

        public void MakePayment(ITransaction transaction)
        {
            try
            {
                DatabaseHandler dp = new DatabaseHandler();
                dp.SaveRequestlog(transaction.VendorCode, transaction.UtilityCode, "POSTING", transaction.CustRef, transaction.Password);
                PostResponse resp = new PostResponse();
                var paymentFactory = new PaymentFactory();
                var payment = paymentFactory.initialisePayment(transaction.UtilityCode);
                var response = payment.pay(transaction);

                Console.WriteLine(resp.Serialize(response));
                Console.ReadLine();
            }
            catch (SqlException)
            {

                Console.WriteLine("Failed to log request!!!"); ;
            }
            catch (Exception)
            {
                Console.WriteLine("GENERAL ERROR AT PEGPAY!!!");
            }
            
        }

        public void MakeDSTVPayment(DSTVTransaction transaction)
        {
            try
            {
                DatabaseHandler dp = new DatabaseHandler();
                dp.SaveRequestlog(transaction.VendorCode, transaction.UtilityCode, "POSTING", transaction.CustRef, transaction.Password);
                PostResponse resp = new PostResponse();
                var paymentFactory = new PaymentFactory();
                var payment = paymentFactory.initialisePayment(transaction.UtilityCode);
                var response = payment.pay(transaction);

                Console.WriteLine(resp.Serialize(response));
                Console.ReadLine();
            }
            catch (SqlException)
            {

                Console.WriteLine("Failed to log request!!!"); 
            }
            catch (Exception)
            {
                Console.WriteLine("GENERAL ERROR AT PEGPAY!!!");
            }

        }
    }
}
