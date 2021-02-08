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
    public abstract class Payment 
    {
        public PostResponse resp = new PostResponse();
        protected DatabaseHandler dp = new DatabaseHandler();
        protected BusinessLogic bll = new BusinessLogic();
        protected PhoneValidator pv = new PhoneValidator();



        protected virtual bool CheckForEmptyProperties(NWSCTransaction trans)
        {
            if (string.IsNullOrEmpty(trans.CustomerType))
            {
                trans.CustomerType = "";
            }
            if (trans.CustomerTel == null)
            {
                trans.CustomerTel = "";
            }
            if (trans.Email == null)
            {
                trans.Email = "";
            }
            if (trans.CustName == null || trans.CustName.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "13", "");
                return false;
            }

            else if (trans.Area == null || trans.Area.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "35", "");
                return false;

            }

            else if (trans.TransactionType == null || trans.TransactionType.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "14", "");
                return false;
            }

            else if (trans.VendorTransactionRef == null || trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "16", "");
                return false;
            }

            else if (trans.Teller == null || trans.Teller.Trim().Equals(""))
            {
                resp.HandleResponse(trans, resp, "17", "");
                return false;
            }

            else if (trans.DigitalSignature == null || trans.DigitalSignature.Length == 0)
            {
                resp.HandleResponse(trans, resp, "19", "");
                return false;
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.HandleResponse(trans, resp, "22", "");
                return false;
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.HandleResponse(trans, resp, "22", "");
                return false;
            }
            else if ((trans.Reversal == "1" && trans.Narration == null) || (trans.Reversal == "1" && trans.Narration.Equals("")))
            {
                resp.HandleResponse(trans, resp, "23", "");
                return false;
            }

            return true;
        }
        protected void CheckTranValidity(Transaction tran)
        {

        }
       
        public bool isValidVendorTraficIpAccess()
        {
            throw new NotImplementedException();
        }

        public bool isValidVendorUtilityMapping()
        {
            throw new NotImplementedException();
        }
        
        protected string GetReversalState(ITransaction trans)
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

        

        
    }
}
