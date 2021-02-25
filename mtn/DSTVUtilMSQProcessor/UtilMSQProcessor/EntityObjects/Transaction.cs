using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using UtilMSQProcessor.ControlObjects;
using UtilMSQProcessor.PegPayLevel1Api;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;

namespace UtilMSQProcessor.EntityObjects
{
    public class Transaction
    {
        string tranId, queueId, statusDescription, digitalSignature, statusCode, paymentDate, password, tranAmount, teller, vendorCode, tranNarration, vendorTranId, tranIdToReverse, paymentType, tranType, customerRef, customerName, customerType, customerTel, reversal, offline, queueTime;


        public string QueueId
        {
            get { return queueId; }
            set { queueId = value; }
        }

        public string QueueTime
        {
            get { return queueTime; }
            set { queueTime = value; }
        }

        public bool IsValidTransaction()
        {
            //STK Payment
            if (Area == customerRef && Area.Length > 6)
            {
                Area = "TOP_UP";
            }
            if (!IsValidCustomer())
            {
                //we failed to get customer details from db
                return false;
            }
            //else
            else if (!CustomerPickedValidBouquetForPayTv())
            {
                StatusCode = "100";
                StatusDescription = "CUSTOMER HAS CHOSEN WRONG BOUQUET FOR THEIR PAYTV";
                return false;
            }
            else if (!IsValidBouquetCode(Area))
            {
                StatusCode = "100";
                StatusDescription = "INVALID BOUQUET CODE PASSED IN TRANSACTION";
                return false;
            }


            StatusCode = "0";
            StatusDescription = "SUCCESS";
            return true;
        }


        private bool IsValidBouquetCode(string bouquetCode)
        {
            DatabaseHandler dh = new DatabaseHandler();
            DataTable dt = dh.GetBouquetByBouquetCode(bouquetCode);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            //box office payment
            if (bouquetCode == "BO")
            {
                return true;
            }
            //box office payment
            else if (bouquetCode == "BOX")
            {
                return true;
            }
            //if this is a MTN STK transaction
            else if (CustomerRef == Area)
            {
                Area = "TOP_UP";
                return true;
            }
            //if this is a MTN STK transaction
            else if (Area == "")
            {
                Area = "TOP_UP";
                return true;
            }
            //box office payment
            else if (bouquetCode == "TOP_UP")
            {
                return true;
            }

            return false;
        }

        private bool CustomerPickedValidBouquetForPayTv()
        {
            if (!string.IsNullOrEmpty(CustomerType) && !string.IsNullOrEmpty(Area))
            {
                //STK Payment
                if (Area == "TOP_UP")
                {
                    return true;
                }
                //if its a go tv dude
                if (CustomerType.ToUpper().Equals("GOTV"))
                {
                    //go tv customer has picked dstv bouquet
                    if (IsNumeric(Area) && Area == "789012")
                    {
                        return true;
                    }
                    if (!Area.ToUpper().StartsWith("GO"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                //its a dstv dude
                else if (CustomerType.ToUpper().Equals("DSTV"))
                {
                    //dstv customer has chosen go tv bouquet
                    if (Area.ToUpper().StartsWith("GO"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                //unknown pay tv dude: probabaly a hacker
                else
                {
                    //unknown customer type has chosen a bouquet
                    return false;
                }
            }
            //bouquet code or customer type is null
            return true;
        }

        //private bool CustomerPickedValidBouquetForPayTv()
        //{
        //    if (!string.IsNullOrEmpty(CustomerType) && !string.IsNullOrEmpty(Area))
        //    {
        //        //if its a go tv dude
        //        if (CustomerType.ToUpper().Equals("GOTV"))
        //        {
        //            //go tv customer has picked dstv bouquet
        //            if (IsNumeric(Area))
        //            {
        //            }
        //            else if (!Area.ToUpper().StartsWith("GO"))
        //            {
        //                return false;
        //            }
        //        }
        //        //its a dstv dude
        //        else if (CustomerType.ToUpper().Equals("DSTV"))
        //        {
        //            //dstv customer has chosen go tv bouquet
        //            if (IsNumeric(Area))
        //            {
        //            }
        //            else if (Area.ToUpper().StartsWith("GO"))
        //            {
        //                return false;
        //            }
        //        }
        //        //unknown pay tv dude: probabaly a hacker
        //        else
        //        {
        //            //unknown customer type has chosen a bouquet
        //            return false;
        //        }
        //    }
        //    //bouquet code or customer type is null
        //    return true;
        //}

        public bool IsNumeric(string amount)
        {
            if (amount.Equals("0"))
            {
                return false;
            }
            else
            {
                //double amt = double.Parse(amount);
                //amount = amt.ToString();
                float Result;
                return float.TryParse(amount, out Result);
            }
        }

        private bool IsValidCustomer()
        {
            DatabaseHandler dh = new DatabaseHandler();
            Customer cust = dh.GetCustDetails(CustomerRef, "DSTV",Area);
            if (cust.StatusCode.Equals("0"))
            {
                CustomerName = cust.CustomerName;
                CustomerType = cust.CustomerType;
                StatusCode = "0";
                StatusDescription = "SUCCESS";
                //return true;
                //this change has been made to enable the topup payment change
                double balance, tranAmount;
                if (CustomerRef == Area || Area == "BO" || Area == "TOP_UP" || IsNumeric(Area))
                {
                    if (cust.CustomerType == "DSTV")
                    {
                        if (Area == "BO")
                        {
                            
                        }
                        else if (Area == "TOP_UP" && Area.Length > 6)
                        {

                        }
                        else
                        {
                            Area = dh.GetSystemParameter(9, 11);

                            if (string.IsNullOrEmpty(Area))
                                Area = "123456";// Topup code for dstv\
                        }
                    }
                    else
                    {
                        Area = dh.GetSystemParameter(9, 10);
                        if (string.IsNullOrEmpty(Area))
                            Area = "789012";
                    }
                    return true;
                }
                
                else if (double.TryParse(cust.Balance, out balance) && double.TryParse(TranAmount, out tranAmount))
                {
                    if (tranAmount >= balance)
                    {
                        return true;
                    }
                    else
                    {

                        StatusCode = "100";
                        StatusDescription = "Insufficent transaction amount";
                        return false;
                    }
                }
                return false;
            }
            else
            {
                //StatusCode = "100";
                //StatusDescription = "CustomerRef not in database: Did customer validate?";
                //return false;

                Response queryResp = QueryForCustomerFromUtilitiesAPI();
                if (queryResp.ResponseField6.Equals("0"))
                {
                    CustomerName = queryResp.ResponseField2;
                    CustomerType = queryResp.ResponseField5;
                    if (Area == CustomerRef)
                    {
                        Area = "TOP_UP";
                    }
                    StatusCode = "0";
                    StatusDescription = "SUCCESS";
                    return true;
                }
                else if (queryResp.ResponseField7.Contains("MULTCHOICE"))
                {

                    StatusCode = "0";
                    StatusDescription = "SUCCESS";
                    return true;


                }
                else
                {
                    CustomerRef = CustomerRef;
                    StatusCode = "100";
                    StatusDescription = queryResp.ResponseField7;
                    return false;
                }
            }
        }
        public const int QUERY_API_TIMEOUT = 1000 * 30;

        private Response QueryForCustomerFromUtilitiesAPI()
        {
            QueryRequest queryReq = new QueryRequest();
            Response resp = new Response();
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                DatabaseHandler dh = new DatabaseHandler();

                queryReq.QueryField1 = customerRef;
                queryReq.QueryField2 = "";
                queryReq.QueryField3 = "";
                queryReq.QueryField4 = "DSTV";
                queryReq.QueryField5 = dh.GetSystemParameter(9, 1);
                queryReq.QueryField6 = dh.GetSystemParameter(9, 2);
                PegPay utilitiesAPI = new PegPay();
                utilitiesAPI.Timeout = QUERY_API_TIMEOUT;
                resp = utilitiesAPI.QueryCustomerDetails(queryReq);

            }
            catch (WebException ex)
            {
                if (ex.Message.ToUpper().Contains("TIMED OUT"))
                {
                    resp.ResponseField6 = "100";
                    resp.ResponseField7 = "VALIDATION TIMEOUT ON LEVEL 1 (55.3):BEYOND " + QUERY_API_TIMEOUT + " SECONDS";
                }
                else
                {
                    throw ex;
                }
            }
            return resp;

        }

        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }




        public string Area;

        public string TranId
        {
            get
            {
                return tranId;
            }
            set
            {
                tranId = value;
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
        public string DigitalSignature
        {
            get
            {
                return digitalSignature;
            }
            set
            {
                digitalSignature = value;
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
        public string TranAmount
        {
            get
            {
                return tranAmount;
            }
            set
            {
                tranAmount = value;
            }
        }
        public string Teller
        {
            get
            {
                return teller;
            }
            set
            {
                teller = value;
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
        public string TranNarration
        {
            get
            {
                return tranNarration;
            }
            set
            {
                tranNarration = value;
            }
        }
        public string VendorTranId
        {
            get
            {
                return vendorTranId;
            }
            set
            {
                vendorTranId = value;
            }
        }
        public string TranIdToReverse
        {
            get
            {
                return tranIdToReverse;
            }
            set
            {
                tranIdToReverse = value;
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
        public string TranType
        {
            get
            {
                return tranType;
            }
            set
            {
                tranType = value;
            }
        }
        public string CustomerRef
        {
            get
            {
                return customerRef;
            }
            set
            {
                customerRef = value;
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
        public string CustomerType
        {
            get
            {
                return customerType;
            }
            set
            {
                customerType = value;
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

        internal bool IsSTKPayment()
        {
            if (Area == customerRef)
            {
                return true;
            }
            return false;
        }
    }
}
