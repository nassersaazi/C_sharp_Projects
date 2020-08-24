using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoggingAPI.Models
{
    public class Logging
    {
        public int id { get; set; } 
        public string date { get; set; }

        public Logging(int id, string date)
        {
            this.id = id;
            this.date = date;
        }
    }
}