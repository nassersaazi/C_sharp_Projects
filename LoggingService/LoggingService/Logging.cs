﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;            
using System.Timers;

namespace LoggingService
{
    public class Logging
    {
        private readonly Timer _timer;

        public Logging()
        {
            _timer = new Timer(5000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
            
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial" +
                " Catalog=sp;Integrated Security=True"; //string of database source we are connecting to

            
            string lines = DateTime.Now.ToString()  ;
            
            using (SqlConnection conn = new SqlConnection(connectionString))//using directive helps close 
            //the connection in case an error occurs 
            {
                conn.Open(); //open connection
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Execute spLogs @date"; //stored procedure to save details
                
                cmd.Parameters.Add("@date", SqlDbType.VarChar, 50).Value =  lines;
                cmd.ExecuteNonQuery();
                
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
