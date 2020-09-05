using sampleTDDAppLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace sampleTDDAppLibrary.Logic
{
    public abstract class Payment : IVendorUtilityAccess
    {
        public PostResponse resp = new PostResponse();
        protected DatabaseHandler dp = new DatabaseHandler();
        protected BusinessLogic bll = new BusinessLogic();
        protected PhoneValidator pv = new PhoneValidator();

        protected void  CheckForEmptyProperties(NWSCTransaction trans)
        {
            if (trans.CustName == null || trans.CustName.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "13", "");
            }

            else if (trans.Area == null || trans.Area.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "35", "");
            }

            else if (trans.TransactionType == null || trans.TransactionType.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "14", "");
            }

            else if (trans.VendorTransactionRef == null || trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "16", "");
            }

            else if (trans.Teller == null || trans.Teller.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "17", "");
            }

            else if (trans.DigitalSignature == null || trans.DigitalSignature.Length == 0)
            {
                resp.HandleResponse(trans, resp, "19", "");
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.HandleResponse(trans, resp, "22", "");
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.HandleResponse(trans, resp, "22", "");
            }
            else if ((trans.Reversal == "1" && trans.Narration == null) || (trans.Reversal == "1" && trans.Narration.Equals("")))
            {
                resp.HandleResponse(trans, resp, "23", "");
            }
        }
        protected void CheckTranValidity(Transaction tran)
        {

        }
        public abstract PostResponse pay(NWSCTransaction tran);
        public bool isSignatureValid(Transaction trans)
        {
            Signature sign = new Signature();
            return sign.VerifySignature(trans);
            
        }


        public bool isActiveVendor(string vendorCode, DataTable vendorData)
        {
            try
            {
                bool activeVendor = bool.Parse(vendorData.Rows[0]["Active"].ToString());
                return activeVendor == true ? true : false;
                
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public virtual bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData)
        {
            Console.WriteLine("IsValidVendorCredentials from parent class\n");
            bool valid = false;
            try
            {
                BusinessLogic bll = new BusinessLogic();
                if (vendorData.Rows.Count != 0)
                {
                    string vendor = vendorData.Rows[0]["VendorCode"].ToString();
                    string encVendorPassword = vendorData.Rows[0]["VendorPassword"].ToString();
                    valid = (vendor.Trim().Equals(vendorCode.Trim()) && encVendorPassword.Trim().Equals(password.Trim())) ? true : false;
                    
                }
                else
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                return valid;
            }
            return valid;
        }

       

        public bool isValidVendorTraficIpAccess()
        {
            throw new NotImplementedException();
        }

        public bool isValidVendorUtilityMapping()
        {
            throw new NotImplementedException();
        }

        public bool IsduplicateVendorRef(Transaction trans)
        {
            bool ret = false;
            DatabaseHandler dp = new DatabaseHandler();
            DataTable dt = dp.GetDuplicateVendorRef(trans);
            if (dt.Rows.Count > 0)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }
            return ret;
        }

        public bool IsValidReversalStatus(Transaction trans)
        {
            if (trans.Reversal == null)
            {
                return false;
            }
            else
            {
                if (trans.Reversal.Equals("0") || trans.Reversal.Equals("1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        

        

        protected bool ReverseAmountsMatch(Transaction trans)
        {
            DatabaseHandler dp = new DatabaseHandler();
            if (trans.Reversal.Equals("0"))
            {
                return true;
            }
            else
            {
                DataTable dt = dp.GetOriginalVendorRef(trans);
                if (dt.Rows.Count > 0)
                {
                    double amount = double.Parse(trans.TransactionAmount);
                    double amountToreverse = double.Parse(dt.Rows[0]["TranAmount"].ToString());
                    double total = amountToreverse + amount;
                    return total.Equals(0) ? true : false;
                   
                }
                else
                {
                    return false;
                }
            }
        }

        protected bool HasOriginalEntry(Transaction trans)
        {
            DatabaseHandler dp = new DatabaseHandler();
            if (trans.Reversal.Equals("0"))
            {
                return true;
            }
            else
            {
                DataTable dt = dp.GetOriginalVendorRef(trans);
                return (dt.Rows.Count > 0) ? true : false;
                
            }
        }

        protected string GetReversalState(Transaction trans)
        {
            string res = "";
            if (trans.Reversal != null)
            {
                double amt = double.Parse(trans.TransactionAmount);
                string amountstr = amt.ToString();
                int amount = int.Parse(amountstr);
                res = (amount > 0) ? "0" : "1";
                
            }
            return res;
        }

        

        protected bool IsduplicateCustPayment(Transaction trans)
        {
            if (trans.VendorCode.Trim().ToUpper() == "MTN")
            {
                return false;
            }
            bool ret = false;
            DatabaseHandler dp = new DatabaseHandler();
            string custRef = trans.CustRef;
            double amount = double.Parse(trans.TransactionAmount);
            DateTime postDate = DateTime.Now;
            DataTable dt = dp.GetDuplicateCustPayment(trans.VendorCode, custRef, amount, postDate);
            if (dt.Rows.Count > 0)
            {
                DateTime Postdate = DateTime.Parse(dt.Rows[0]["RecordDate"].ToString());
                TimeSpan t = postDate.Subtract(Postdate);
                int tmdiff = t.Minutes;
                ret = (tmdiff < 10) ? true : false;
                
            }
            else
            {
                ret = false;
            }
            return ret;
        }
    }
}
