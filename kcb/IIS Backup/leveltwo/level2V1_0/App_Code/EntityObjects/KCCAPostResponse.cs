using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for KCCAPostResponse
/// </summary>
public class KCCAPostResponse : UmemePostResponse
{
    private string SessionKey, Reference, Hash, Backref, TransactionId,
         TrackingId, Status, paymentDate, paymentType, paymentReference, paymentDescription,
         paymentGroup, paymentAmount, paymentCurrency, customerID, customerName, customerPhone,
         customerRef, outstandingBalance, errorCode;

    private string chequeNumber, narration, custRef, custName, area, tin, customerTel, vendorTransactionRef,
             transactionType, vendorCode, password, companyCode, utilityCompany, bank, branchCode,
             collectionArea, collectionBranch, recordedby, accountNumber, tellerAccount,
             course, yearsem, reversal, reversedTrans, status_code, offline, username, coin, key, secretKey;

    private int transactionAmount, recordId, transactionCharge, billAmount;
    private bool sent, fileGenerated;

    public string status
    {
        get
        {
            return Status;
        }
        set
        {
            Status = value;
        }
    }

    public string SecretKey
    {
        get
        {
            return secretKey;
        }
        set
        {
            secretKey = value;
        }
    }

    public string Key
    {
        get
        {
            return key;
        }
        set
        {
            key = value;
        }
    }

    public string Error_code
    {
        get
        {
            return errorCode;
        }
        set
        {
            errorCode = value;
        }
    }
    public string CustomerPhone
    {
        get
        {
            return customerPhone;
        }
        set
        {
            customerPhone = value;
        }
    }
    public string CustomerID
    {
        get
        {
            return customerID;
        }
        set
        {
            customerID = value;
        }
    }
    public string CustomerName
    {
        get
        {
            return customerName;
        }
        set
        {
            customerName = value;
        }
    }
    public string PaymentCurrency
    {
        get
        {
            return paymentCurrency;
        }
        set
        {
            paymentCurrency = value;
        }
    }
    public string PaymentGroup
    {
        get
        {
            return paymentGroup;
        }
        set
        {
            paymentGroup = value;
        }
    }
    public string PaymentAmount
    {
        get
        {
            return paymentAmount;
        }
        set
        {
            paymentAmount = value;
        }
    }
    public string PaymentDescription
    {
        get
        {
            return paymentDescription;
        }
        set
        {
            paymentDescription = value;
        }
    }
    public string PaymentReference
    {
        get
        {
            return paymentReference;
        }
        set
        {
            paymentReference = value;
        }
    }
    public string PaymentType
    {
        get
        {
            return paymentType;
        }
        set
        {
            paymentType = value;
        }
    }
    public string PaymentDate
    {
        get
        {
            return paymentDate;
        }
        set
        {
            paymentDate = value;
        }
    }
    public string TransID
    {
        get
        {
            return TransactionId;
        }
        set
        {
            TransactionId = value;
        }
    }
    public string TrackingID
    {
        get
        {
            return TrackingId;
        }
        set
        {
            TrackingId = value;
        }
    }
    public string backRef
    {
        get
        {
            return Backref;
        }
        set
        {
            Backref = value;
        }
    }
    public string hash
    {
        get
        {
            return Hash;
        }
        set
        {
            Hash = value;
        }
    }
    public string sessionKey
    {
        get
        {
            return SessionKey;
        }
        set
        {
            SessionKey = value;
        }
    }


    public string Coin
    {
        get
        {
            return coin;
        }
        set
        {
            coin = value;
        }
    }
    public string Offline
    {
        get
        {
            return offline;
        }
        set
        {
            offline = value;
        }
    }
    public string CustRef
    {
        get
        {
            return custRef;
        }
        set
        {
            custRef = value;
        }
    }
    public string Reversal
    {
        get
        {
            return reversal;
        }
        set
        {
            reversal = value;
        }
    }
    public string ReversedTrans
    {
        get
        {
            return reversedTrans;
        }
        set
        {
            reversedTrans = value;
        }
    }
    public string Course
    {
        get
        {
            return course;
        }
        set
        {
            course = value;
        }
    }
    public string YearSem
    {
        get
        {
            return yearsem;
        }
        set
        {
            yearsem = value;
        }
    }

    public string TellerAccount
    {
        get
        {
            return tellerAccount;
        }
        set
        {
            tellerAccount = value;
        }
    }
    public string AccountNumber
    {
        get
        {
            return accountNumber;
        }
        set
        {
            accountNumber = value;
        }
    }
    public string CollectionArea
    {
        get
        {
            return collectionArea;
        }
        set
        {
            collectionArea = value;
        }
    }
    public string CollectionBranch
    {
        get
        {
            return collectionBranch;
        }
        set
        {
            collectionBranch = value;
        }
    }
    public bool Sent
    {
        get
        {
            return sent;
        }
        set
        {
            sent = value;
        }
    }
    public bool FileGenerated
    {
        get
        {
            return fileGenerated;
        }
        set
        {
            fileGenerated = value;
        }
    }
    public string BranchCode
    {
        get
        {
            return branchCode;
        }
        set
        {
            branchCode = value;
        }
    }
    public string Bank
    {
        get
        {
            return bank;
        }
        set
        {
            bank = value;
        }
    }
    public string UtilityCompany
    {
        get
        {
            return utilityCompany;
        }
        set
        {
            utilityCompany = value;
        }
    }
    public string CompanyCode
    {
        get
        {
            return companyCode;
        }
        set
        {
            companyCode = value;
        }
    }
    public string Narration
    {
        get
        {
            return narration;
        }
        set
        {
            narration = value;
        }
    }

    public string Tin
    {
        get
        {
            return tin;
        }
        set
        {
            tin = value;
        }
    }
    public string ChequeNumber
    {
        get
        {
            return chequeNumber;
        }
        set
        {
            chequeNumber = value;
        }
    }
    public string CustName
    {
        get
        {
            return custName;
        }
        set
        {
            custName = value;
        }
    }
    public string Area
    {
        get
        {
            return area;
        }
        set
        {
            area = value;
        }
    }
    public string CustomerTel
    {
        get
        {
            return customerTel;
        }
        set
        {
            customerTel = value;
        }
    }
    public string VendorTransactionRef
    {
        get
        {
            return vendorTransactionRef;
        }
        set
        {
            vendorTransactionRef = value;
        }
    }
    public string TransactionType
    {
        get
        {
            return transactionType;
        }
        set
        {
            transactionType = value;
        }
    }
    public string VendorCode
    {
        get
        {
            return vendorCode;
        }
        set
        {
            vendorCode = value;
        }
    }
    public string Password
    {
        get
        {
            return password;
        }
        set
        {
            password = value;
        }
    }

    public int TransactionAmount
    {
        get
        {
            return transactionAmount;
        }
        set
        {
            transactionAmount = value;
        }
    }
    public int TransactionCharge
    {
        get
        {
            return transactionCharge;
        }
        set
        {
            transactionCharge = value;
        }
    }
    public int BillAmount
    {
        get
        {
            return billAmount;
        }
        set
        {
            billAmount = value;
        }
    }
    public int RecordId
    {
        get
        {
            return recordId;
        }
        set
        {
            recordId = value;
        }
    }

    public string RecordedBy
    {
        get
        {
            return recordedby;
        }
        set
        {
            recordedby = value;
        }
    }

    public string Username
    {
        get
        {
            return username;
        }
        set
        {
            username = value;
        }
    }

}
