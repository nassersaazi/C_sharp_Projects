using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sampleTDDAppLibrary.Logic
{
    public class NWSCPayment : Payment,IPayment
    {
        public PostResponse pay(ITransaction trans)
        {
           
            
            string vendorCode = trans.VendorCode;
           

            if (CheckForEmptyProperties((NWSCTransaction)trans))
            {
                try
                {
                    DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                    Validator validator = new Validator();

                    var response =  validator.validate((NWSCTransaction)trans, resp, vendaData);

                    if (response != null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = response.StatusCode;
                        resp.StatusDescription = response.StatusDescription;
                        return resp;
                    }

                    trans.Reversal = GetReversalState(trans);
                    
                   
                    resp.HandleResponse(trans, resp, "0", "");
                 
                    
                    if (resp.StatusCode.Equals("2"))
                    {
                        DataTable dt = dp.GetVendorDetails(vendorCode);
                        if (dt.Rows.Count != 0)
                        {
                            string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                            int loginCount = int.Parse(strLoginCount);
                            loginCount = loginCount + 1;
                            if (loginCount == 3)
                            {
                                dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                                dp.DeactivateVendor(vendorCode, ipAddress);
                            }
                            {
                                dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                            }
                        }
                    }
                }
                catch (System.Net.WebException wex)
                {

                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                    dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "NWSC");
                }
                catch (SqlException sqlex)
                {
                    resp.StatusCode = "31";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
                    resp.PegPayPostId = "";
                }
                catch (Exception ex)
                {
                    resp.StatusCode = "32";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
                    resp.PegPayPostId = "";
                    dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "NWSC");
                } 
            }
            return resp;
        }

       

        
    }
}
