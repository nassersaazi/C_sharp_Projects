using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace LoggingAPI
{
    public static class WebApiConfig
    {
        public static SqlConnection conn()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial" +
                " Catalog=sp;Integrated Security=True"; //string of database source we are connecting to

            SqlConnection conn = new SqlConnection(connectionString);
            return conn;
        }

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
