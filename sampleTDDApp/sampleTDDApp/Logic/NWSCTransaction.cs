using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDApp.Logic
{
    public class NWSCTransaction : Transaction
    {
        public string area, UtilityCompany;

        public string utilityCompany
        {
            get
            {
                return UtilityCompany;
            }
            set
            {
                UtilityCompany = value;
            }
        }

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
