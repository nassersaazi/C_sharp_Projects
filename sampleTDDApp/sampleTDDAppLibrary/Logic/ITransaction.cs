using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public interface ITransaction
    {
        string ChequeNumber { get; set; }
        string Narration { get; set; }
        string CustRef { get; set; }
        string CustName { get; set; }
        string CustomerTel { get; set; }
        string VendorTransactionRef { get; set; }
        string TransactionType { get; set; }
        string VendorCode { get; set; }
        string Password { get; set; }
        string Teller { get; set; }
        string Reversal { get; set; }
        string ReversedTrans { get; set; }
        string Offline { get; set; }
        string UtilityCode { get; set; }
        string PaymentDate { get; set; }
        string TransactionAmount { get; set; }
        string DigitalSignature { get; set; }
        string Telephone { get; set; }
        string Email { get; set; }
        string transactionID { get; set; }
        string Tin { get; set; }
        string Area { get; set; }
        string CustomerType { get; set; }
        string TranIdToReverse { get; set; }
       
    }
}
