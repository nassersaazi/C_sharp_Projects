using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;
using System.IO;
using System.Net;

namespace DSTVListener.ControlObjects
{
    public static class DbLayer
    {
        public static readonly List<Connection> Connections = new List<Connection>();
        public static readonly string DB1 = ".5";
        public static readonly string DB2 = ".7";
        private static string FilePath = @"E:\Pegasus.Config\Configs.csv";

        public static readonly string IP = GetHostIp();
        public static string currentIP = GetServerLocation("Server");

        private static string GetHostIp()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ip = Dns.GetHostByName(strHostName);
            IPAddress[] IPaddr = ip.AddressList;
            return IPaddr[0].ToString();
        }

        public static Database CreateDatabase(string configName, string searchValue)
        {
            Connection con = new Connection(configName, searchValue);
            if (con.AlreadyExists())
            {
                return con.db;
            }
            else
            {
                string conString = ConfigurationManager.ConnectionStrings[con.Name].ConnectionString;
                con.Server = GetServerLocation(con.SearchName);
                //if (con.Server.Equals("DBSERVER2")) { con.Server = "192.168.0.7"; }
                conString = string.Format(conString, con.Server);
                con.db = new SqlDatabase(conString);
                Add(con);
                return con.db;
            }
        }

        private static void Add(Connection con)
        {
            if (!DbLayer.Connections.Contains(con))
            {
                DbLayer.Connections.Add(con);
            }
        }

        private static string GetServerLocation(string searchValue)
        {
            string server = "";
            string[] fileTransactions = File.ReadAllLines(FilePath);

            if (fileTransactions.Length > 0)
            {
                foreach (string str in fileTransactions)
                {
                    string searchname = "";
                    string servername = "";
                    string[] data = str.Split(',');
                    if (data.Length > 0)
                    {
                        searchname = data[0];
                        servername = data[1];
                    }
                    if (searchname.Equals(searchValue))
                    {
                        server = servername;
                        break;
                    }
                }
            }

            return server;
        }

    }

    public class Connection
    {
        public string Name = "";
        public string Server = "";
        public string SearchName = "";
        public Database db;

        public Connection(string configName, string searchName)
        {
            this.Name = configName;
            this.SearchName = searchName;
        }

        internal bool AlreadyExists()
        {
            bool valid = false;

            foreach (Connection con in DbLayer.Connections)
            {
                if (this.Name == con.Name && this.SearchName == con.SearchName && con.db != null)
                {
                    this.Server = con.Server;
                    this.db = con.db;
                    valid = true;
                }
            }
            return valid;
        }
    }
}
