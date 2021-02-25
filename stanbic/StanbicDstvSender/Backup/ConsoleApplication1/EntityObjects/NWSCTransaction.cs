using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1.EntityObjects
{
    public class NWSCTransaction : Transaction
    {
        private string area;

        public string Area
        {
            get
            {
                return area;
            }
            set
            {
                area = value;
            }
        }
    }
}
