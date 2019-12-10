using LoggingAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LoggingAPI.Controllers
{
    public class LoggingController : ApiController
    {
        List<Logging> logs = new List<Logging>();

      //  public LoggingController()
        //{
          //  logs.Add(new Logging { Date = "2/3/19" ,Id = 1 });
            //logs.Add(new Logging { Date = "12/3/19", Id = 2 });
            //logs.Add(new Logging { Date = "23/2/19", Id = 3 });
        //}

        // GET: api/Logging
        public List<Logging> Get()
        {
            SqlConnection conn = WebApiConfig.conn();

            conn.Open(); //open connection
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM logs";

            SqlDataReader fetch = query.ExecuteReader();

            while (fetch.Read())
            {
                logs.Add(new Logging(Convert.ToInt32(fetch["id"]), fetch["date"].ToString()));

            }

            return logs;
        }

        // GET: api/Logging/5
        public List<Logging> Get(int id)
        {
            SqlConnection conn = WebApiConfig.conn();

            conn.Open(); //open connection
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM logs";//select * from tbl_name order by id desc limit N;

            SqlDataReader fetch = query.ExecuteReader();

            while (fetch.Read())
            {
                logs.Add(new Logging(Convert.ToInt32(fetch["id"]), fetch["date"].ToString()));
               
            }

            return logs;
            // return logs.Where(x => x.Id == id).FirstOrDefault();
        }

        // POST: api/Logging
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Logging/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Logging/5
        public void Delete(int id)
        {
        }
    }
}
