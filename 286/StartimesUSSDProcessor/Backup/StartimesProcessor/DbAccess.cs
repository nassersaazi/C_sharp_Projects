using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace StartimesProcessor
{
    public class DbAccess
    {
        DbApi.DataAccess access = new DbApi.DataAccess();

        internal DataSet ExecuteDataSet(string procedure, params object[] parameters)
        {
            return access.ExecuteDataSet("5", "LivePegasusUssd", procedure, parameters);
        }

        internal DbApi.Result ExecuteNonQuery(string procedure, params object[] parameters)
        {
            return access.ExecuteNonQuery("5", "LivePegasusUssd", procedure, parameters);
        }
    }
}
