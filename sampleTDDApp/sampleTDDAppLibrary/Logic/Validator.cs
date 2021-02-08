using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public class Validator: IVendorUtilityAccess, ITransactionValidity
    {
        public bool isSignatureValid(ITransaction trans)
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

        public  bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData)
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
        public bool IsValidDate(string paymentDate)
        {
            DateTime date;
            string format = "dd/MM/yyyy";
            return DateTime.TryParseExact(paymentDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

        }

        public bool IsValidReversalStatus(ITransaction trans)
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

        public bool IsNumeric(string amount)
        {

            if (amount.Equals("0"))
            {
                return false;
            }
            else
            {
                double amt = double.Parse(amount);
                amount = amt.ToString();
                float Result;
                return float.TryParse(amount, out Result);
            }
        }

        public bool IsduplicateVendorRef(ITransaction trans)
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

        protected bool IsduplicateCustPayment(ITransaction trans)
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

        public PostResponse validate(NWSCTransaction trans,PostResponse resp,DataTable vendaData)
        {
            PhoneValidator pv = new PhoneValidator();
            DatabaseHandler dp = new DatabaseHandler();

            resp = null;
            UtilityCredentials creds = dp.GetUtilityCreds("NWSC", trans.VendorCode);
            string vendorType = vendaData.Rows[0]["VendorType"].ToString();

            if (!IsValidReversalStatus(trans))
            {
                resp.HandleResponse(trans, resp, "25", "");
                return resp;
            }

            else if (!IsNumeric(trans.TransactionAmount))
            {
                resp.HandleResponse(trans, resp, "3", "");
                return resp;
            }
            else if (!IsValidDate(trans.PaymentDate))
            {
                resp.HandleResponse(trans, resp, "4", "");
                return resp;
            }

            else if (!isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
            {
                resp.HandleResponse(trans, resp, "2", "");
                return resp;
            }
            else if (!isActiveVendor(trans.VendorCode, vendaData))
            {
                resp.HandleResponse(trans, resp, "11", "");
                return resp;
            }
            else if (!isSignatureValid(trans))
            {
                resp.HandleResponse(trans, resp, "18", "");
                return resp;
            }
            else if (!pv.PhoneNumbersOk(trans.CustomerTel))
            {
                resp.HandleResponse(trans, resp, "12", "");
                return resp;
            }
            else if (IsduplicateVendorRef(trans))
            {
                resp.HandleResponse(trans, resp, "20", "");
                return resp;

            }
            else if (IsduplicateCustPayment(trans))
            {
                resp.HandleResponse(trans, resp, "21", "");
                return resp;
            }

            else if (!HasOriginalEntry(trans))
            {
                resp.HandleResponse(trans, resp, "24", "");
                return resp;
            }
            else if (!dp.ReverseAmountsMatch(trans))
            {
                resp.HandleResponse(trans, resp, "26", "");
                return resp;

            }
            else if (dp.IsChequeBlacklisted(trans))
            {
                resp.HandleResponse(trans, resp, "29", "");
                return resp;

            }

            else if ((vendorType.Equals("PREPAID")))
            {
                resp.HandleResponse(trans, resp, "29", "NOT ENABLED FOR PREPAID VENDORS");
                return resp;

            }

            else if (creds.UtilityCode.Equals(""))
            {
                resp.HandleResponse(trans, resp, "29", "");
                return resp;

            }
            
            else
            {
                return resp;
            }
            
        }

        protected bool HasOriginalEntry(ITransaction trans)
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

        public bool isValidVendorUtilityMapping()
        {
            throw new NotImplementedException();
        }

        public bool isValidVendorTraficIpAccess()
        {
            throw new NotImplementedException();
        }
    }
}
