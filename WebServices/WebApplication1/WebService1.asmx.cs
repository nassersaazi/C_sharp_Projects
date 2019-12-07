using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// Adds two numbers
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [WebMethod]
        public int add(int x, int y)
        {
            return (x + y);
        }


        /// <summary>
        /// Subtracts two numbers
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [WebMethod]
        public int subtract(int x, int y)
        {
            return (x - y);
        }


        /// <summary>
        /// Multiplies two numbers
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [WebMethod]
        public int multiply(int x, int y)
        {
            return (x * y);
        }


        /// <summary>
        /// Divides to numbers, in case result is a fraction ,returns zero
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [WebMethod]
        public int divide(int x, int y)
        {
            return (x / y);
        }
    }
}
