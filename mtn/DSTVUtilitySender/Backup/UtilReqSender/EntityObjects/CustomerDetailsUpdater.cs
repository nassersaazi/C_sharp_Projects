using System;
using System.Collections.Generic;
using System.Text;
using UtilReqSender.ControlObjects;

namespace UtilReqSender.EntityObjects
{
    public class CustomerDetailsUpdater
    {
        Transaction trans;
        public CustomerDetailsUpdater(Transaction trans) 
        {
            this.trans = trans;
        }

        public void DoWork(Object a) 
        {
            try
            {
                Procssor proc = new Procssor();
                proc.UpdateCustomerDetails(trans);
            }
            catch (Exception ex)
            {
                Procssor.log.Info("Exception: "+ex.Message);
            }
        }
    }
}
