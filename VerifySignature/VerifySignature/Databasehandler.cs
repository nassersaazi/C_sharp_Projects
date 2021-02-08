using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifySignature
{
    public class DatabaseHandler
    {
        private Database pegpaydbase;
        private DbCommand command;

        public DatabaseHandler()
        {

            try
            {
                pegpaydbase = DatabaseFactory.CreateDatabase("TestPegPay");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal DataTable GetSystemSettings(string Valuecode, string ValueGroupcode)
        {
            
            try
            {

                string[] parameters = { Valuecode, ValueGroupcode };
                //command = PegPayInterface.GetStoredProcCommand("GetSystemSettings", Valuecode, ValueGroupcode);
                command = pegpaydbase.GetStoredProcCommand("GetSystemSettings", parameters);
                DataSet ds = pegpaydbase.ExecuteDataSet(command);
                DataTable dt = ds.Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal DataSet ExecuteDataSet(string procedure, params object[] parameters)
        {
            command = pegpaydbase.GetStoredProcCommand(procedure, parameters);
            return pegpaydbase.ExecuteDataSet(command);
            //return access.ExecuteDataSet("5", "GenericPegPayApi", procedure, parameters);
        }
    }
}
