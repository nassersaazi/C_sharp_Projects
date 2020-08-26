using kcbTester.EntityObjects;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kcbTester.ControlObjects
{
    public class DatabaseHandler
    {
        //private Database PegPayInterface;
        private DbCommand command;
        private const String constring = "TestPegPay";
        //private DbAccess PegPayInterface = new DbAccess();
        private Database pegpaydbase;

        public DatabaseHandler()
        {
            try
            {
                pegpaydbase = DatabaseFactory.CreateDatabase(constring);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        internal Hashtable GetNetworkRates()
        {
            Hashtable networkRates = new Hashtable();
            try
            {

                string[] parameters = { };
                DataSet ds = pegpaydbase.ExecuteDataSet("GetNetworkRates", parameters);
                //DataSet ds = PegPayInterface.ExecuteDataSet(command);
                int recorcCount = ds.Tables[0].Rows.Count;
                for (int i = 0; i < recorcCount; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string network = dr["Network"].ToString();
                    int rate = int.Parse(dr["Rate(UShs.)"].ToString());
                    networkRates.Add(network, rate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return networkRates;
        }

        internal string GetServerStatus()
        {
            string status = "";
            try
            {
                //command = PegPayInterface.GetStoredProcCommand("GetServerStatus");
                string[] p = { };
                DataTable dt = pegpaydbase.ExecuteDataSet("GetServerStatus", p).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    status = dt.Rows[0]["ServerStatus"].ToString();
                }
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

        internal void SaveDstvCustomerDetails(Customer cust)
        {
            try
            {
                command = pegpaydbase.GetStoredProcCommand("SaveCustomerDetails2", cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, cust.AgentCode, cust.TIN);
                //string[] p ={ cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, cust.AgentCode, cust.TIN };
                int count = pegpaydbase.ExecuteNonQuery(command);//"SaveCustomerDetails2", p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

      
       

        internal Hashtable GetNetworkCodes()
        {
            Hashtable networkCodes = new Hashtable();
            try
            {

                string[] parameters = { };
                DataSet ds = pegpaydbase.ExecuteDataSet("GetNetworkCodes", parameters);
                //DataSet ds = PegPayInterface.ExecuteDataSet(command);
                int recordCount = ds.Tables[0].Rows.Count;
                if (recordCount != 0)
                {
                    for (int i = 0; i < recordCount; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        string network = dr["Network"].ToString();
                        string code = dr["Code"].ToString();
                        networkCodes.Add(code, network);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return networkCodes;
        }

       
        internal void UpdateTransactionStatus(string pegPayReceiptNumber, string Status)
        {
            try
            {
                //command = PegPayInterface.GetStoredProcCommand("UpdateTransactionStatus", pegPayReceiptNumber, Status);
                string[] p = { pegPayReceiptNumber, Status };
                pegpaydbase.ExecuteNonQuery("UpdateTransactionStatus", p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        internal DataTable GetVendorDetails(string vendorCode)
        {
            try
            {
                string[] parameters = { vendorCode };
                //DataTable dt = PegPayInterface.ExecuteDataSet("GetVendorDetails", parameters).Tables[0];
                command = pegpaydbase.GetStoredProcCommand("GetVendorDetails", parameters);
                DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
        internal ArrayList GetBlackListedNumbers()
        {
            ArrayList blacklisted = new ArrayList();
            try
            {
                string[] parameters = { };
                DataSet ds = pegpaydbase.ExecuteDataSet("GetBlacklistedNumbers", parameters);
                //DataSet ds = PegPayInterface.ExecuteDataSet(command);
                int recorcCount = ds.Tables[0].Rows.Count;
                for (int i = 0; i < recorcCount; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string number = dr["Phone"].ToString();
                    blacklisted.Add(number);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return blacklisted;
        }

        private string FormatDate(string date)
        {
            string[] parts = date.Split('/');
            string day = parts[0];
            string month = parts[1];
            string year = parts[2];
            return year + "-" + month + "-" + day;
        }

        

        internal string GetDSTVAccountType(string p)
        {
            string customertype = "";
            try
            {
                DataTable dt = pegpaydbase.ExecuteDataSet("GetCustomerTypes", p).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    customertype = dt.Rows[0][0].ToString();
                }
                else
                {
                    customertype = "UNKNOWN CUSTOMER TYPE: " + p;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in method:GetDSTVAccountType (Method to get DSTV Customer Types in DB)- " + ex.ToString());
            }
            return customertype;
        }

        //public BouquetDetails[] GetBouquetDetailsFromDB(string bouquetCode, string PayTvCode)
        //{
        //    List<BouquetDetails> allBouquets = new List<BouquetDetails>();
        //    try
        //    {
        //        //command = PegPayInterface.GetStoredProcCommand("GetBouquetDetails", bouquetCode, PayTvCode);
        //        string[] p = { bouquetCode, PayTvCode };
        //        DataTable dt = PegPayInterface.ExecuteDataSet("GetBouquetDetails", p).Tables[0];

        //        if (dt.Rows.Count != 0)
        //        {
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                //we are getting details for 1 bouquet
        //                if (!string.IsNullOrEmpty(bouquetCode))
        //                {
        //                    BouquetDetails bd = new BouquetDetails();
        //                    bd.BouquetCode = row["BouquetCode"].ToString();
        //                    bd.BouquetDescription = row["BouquetDescription"].ToString();
        //                    bd.BouquetName = row["BouquetName"].ToString();
        //                    bd.BouquetPrice = row["BouquetPrice"].ToString();
        //                    bd.StatusCode = "0";
        //                    //bd.StatusDescription = "SUCCESS";
        //                    //if ((bd.BouquetCode.ToUpper().Trim() == bouquetCode.ToUpper().Trim())||(bd..ToUpper().Trim() == bouquetCode.ToUpper().Trim()))
        //                    //{
        //                    allBouquets.Add(bd);
        //                    return allBouquets.ToArray();
        //                    //}
        //                }
        //                //we are getting details for all bouquets
        //                else
        //                {
        //                    BouquetDetails bd = new BouquetDetails();
        //                    bd.BouquetCode = row["BouquetCode"].ToString();
        //                    bd.BouquetDescription = row["BouquetDescription"].ToString();
        //                    bd.BouquetName = row["BouquetName"].ToString();
        //                    bd.BouquetPrice = row["BouquetPrice"].ToString();
        //                    bd.StatusCode = "0";
        //                    bd.StatusDescription = "SUCCESS";
        //                    allBouquets.Add(bd);

        //                }
        //            }
        //        }
        //        else
        //        {
        //            allBouquets.Clear();
        //            BouquetDetails bd = new BouquetDetails();
        //            bd.StatusCode = "100";
        //            bd.StatusDescription = "NO BOUQUET DETAILS FOR BOUQUET CODE";
        //            allBouquets.Add(bd);
        //        }
        //        return allBouquets.ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    //return allBouquets.ToArray();

        //}

        //public int SaveBouquetDetails(BouquetDetails bd)
        //{
        //    try
        //    {
        //        //command = PegPayInterface.GetStoredProcCommand("SaveBouquetDetails", bd.BouquetCode, bd.BouquetName, bd.BouquetDescription, bd.BouquetPrice, bd.PayTvCode);
        //        string[] p = { bd.BouquetCode, bd.BouquetName, bd.BouquetDescription, bd.BouquetPrice, bd.PayTvCode };
        //        string strrows = PegPayInterface.ExecuteNonQuery("SaveBouquetDetails", p).RowsAffected;
        //        int rows = Convert.ToInt32(strrows);
        //        return rows;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //internal int SaveReactivateRequest(string smartCardNumber)
        //{
        //    try
        //    {
        //        //command = PegPayInterface.GetStoredProcCommand("SaveReactivateRequest", smartCardNumber);
        //        string[] p = { smartCardNumber };
        //        string rowstr = PegPayInterface.ExecuteNonQuery("SaveReactivateRequest", p).RowsAffected;
        //        int rows = Convert.ToInt32(rowstr);
        //        return rows;
        //    }
        //    catch (Exception ex)
        //    {
        //        return -1;
        //    }
        //}

        //internal string checkCustomerDetails(string custReference)
        //{
        //    string status = "";
        //    DataTable returnTable = new DataTable();
        //    try
        //    {
        //        //command = PegPayInterface.GetStoredProcCommand("GetCustomerDetails", custReference);
        //        string[] p = { custReference };
        //        returnTable = PegPayInterface.ExecuteDataSet("GetCustomerDetails", p).Tables[0];
        //        if (returnTable.Rows.Count > 0)
        //        {
        //            status = "true";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return status;// 
        //}

        internal void SaveRequestlog(string VendorCode, string Utility, string LogType, string CustRef, string password)
        {
            try
            {
                string[] parameters = { VendorCode, Utility, LogType, CustRef, password };
                command = pegpaydbase.GetStoredProcCommand("SaveRequestlog", parameters);
                command.CommandTimeout = 300000000;
                ////PegPayInterface.ExecuteNonQuery("SaveRequestlog", parameters);
                pegpaydbase.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        //internal string LogReversalRequest(ReversalRequest req)
        //{
        //    try
        //    {
        //        string[] parameters ={ req.VendorCode,
        //                           req.OriginalTransactionId,
        //                           req.ReversalTransactionId,
        //                           req.Reason,
        //                           "PENDING",
        //                           "",
        //                           "",
        //                           "" };
        //        DataTable dt = PegPayInterface.ExecuteDataSet("InsertIntoPrepaidReversalRequests", parameters).Tables[0];
        //        string PegPayID = dt.Rows[0][0].ToString();
        //        return PegPayID;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        internal DataTable GetDuplicateReversalRef(string VendorCode, string ReversalTransactionId)
        {
            try
            {
                //command = PegPayInterface.GetStoredProcCommand("GetDuplicateReversalRef", VendorCode, ReversalTransactionId);
                DataTable returndetails = pegpaydbase.ExecuteDataSet("GetDuplicateReversalRef", VendorCode, ReversalTransactionId).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable CheckIfReversed(string VendorCode, string OriginalTransactionId)
        {
            try
            {
                //command = PegPayInterface.GetStoredProcCommand("CheckIfReversedAlready", VendorCode, OriginalTransactionId);
                DataTable returndetails = pegpaydbase.ExecuteDataSet("CheckIfReversedAlready", VendorCode, OriginalTransactionId).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


       
        internal string GetCustomerType(string type)
        {
            string typeName = "";
            try
            {

                string[] parameters = { type };
                DataTable customerType = pegpaydbase.ExecuteDataSet("GetCustomerTypes", parameters).Tables[0];
                if (customerType.Rows.Count > 0)
                {
                    typeName = customerType.Rows[0]["TypeCode"].ToString();

                }
                else
                {
                    typeName = type;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return typeName;
        }
    }
}
