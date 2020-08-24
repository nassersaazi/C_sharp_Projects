using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using myReversalTester.ControlObject;

namespace myReversalTester
{
    class Program
    {


       public static void Main(string[] args)
        {
           
              try
                {
                    Proccesor prog = new Proccesor();
                    prog.RequestedReversal();
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }



        }

        }

