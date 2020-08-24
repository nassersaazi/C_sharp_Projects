using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public static List<IDataConnection> Connection { get; private set; } = new List<IDataConnection>();

        public static void InitializeConnections( DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                // TODO - Set up the Sql Connector properly
                SqlConnector sql = new SqlConnector();
                Connection.Add(sql);

            }

            if (db == DatabaseType.TextFile)
            {
                //TODO -Create the Text Connection
                TextConnector text = new TextConnector();
                Connection.Add(text);
            }
        }
    }
}
