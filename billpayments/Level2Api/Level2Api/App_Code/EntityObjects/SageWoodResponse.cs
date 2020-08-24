using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SageWoodResponse
/// </summary>
public class SageWoodResponse
{

        private string dispHeader, contactNo, clientId, terminlaId, daysOflastPurchase, serverId, accNo, address, locRef, stsscipher, sts, msno, sgc, krn, ti, name;
        private string statusCode, statusDescription, receiptNo, token, units,balance;

        public string DispHeader
        {
            get
            {
                return dispHeader;
            }
            set
            {
                dispHeader = value;
            }
        }

        public string ReceiptNo
        {
            get
            {
                return receiptNo;
            }
            set
            {
                receiptNo = value;
            }
        }
        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }
        public string Units
        {
            get
            {
                return units;
            }
            set
            {
                units = value;
            }
        }
        public string StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                statusCode = value;
            }
        }
        public string StatusDescription
        {
            get
            {
                return statusDescription;
            }
            set
            {
                statusDescription = value;
            }
        }
        public string ClientId
        {
            get
            {
                return clientId;
            }
            set
            {
                clientId = value;
            }
        }
        public string DaysOflastPurchase
        {
            get
            {
                return daysOflastPurchase;
            }
            set
            {
                daysOflastPurchase = value;
            }
        }
        public string ContactNo
        {
            get
            {
                return contactNo;
            }
            set
            {
                contactNo = value;
            }
        }
        public string TerminlaId
        {
            get
            {
                return terminlaId;
            }
            set
            {
                terminlaId = value;
            }
        }
        public string ServerId
        {
            get
            {
                return serverId;
            }
            set
            {
                serverId = value;
            }
        }

        public string AccNo
        {
            get
            {
                return accNo;
            }
            set
            {
                accNo = value;
            }
        }

        public string LocRef
        {
            get
            {
                return locRef;
            }
            set
            {
                locRef = value;
            }
        }

        public string Stsscipher
        {
            get
            {
                return stsscipher;
            }
            set
            {
                stsscipher = value;
            }
        }

        public string Sts
        {
            get
            {
                return sts;
            }
            set
            {
                sts = value;
            }
        }

        public string Sgc
        {
            get
            {
                return sgc;
            }
            set
            {
                sgc = value;
            }
        }

        public string Krn
        {
            get
            {
                return krn;
            }
            set
            {
                krn = value;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        public string Ti
        {
            get
            {
                return ti;
            }
            set
            {
                ti = value;
            }
        }
        public string Msno
        {
            get
            {
                return msno;
            }
            set
            {
                msno = value;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }