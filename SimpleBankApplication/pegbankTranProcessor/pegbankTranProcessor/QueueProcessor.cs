using pegbankTranProcessor.pegbankApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pegbankTranProcessor
{
    public class QueueProcessor
    {
        
        public void DepositsFromQueue()
        {
            pegbank pegpay = new pegbank();
            try
            {

                MessageQueue depositQueue = new MessageQueue(".\\private$\\agentDeposit");

                depositQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Transaction) });
                
                Message msgDeposit = depositQueue.Receive();

                Transaction dep = (Transaction)msgDeposit.Body;

                string deposit = pegpay.DepositFunds(dep);
                Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                throw ex;
                // return "ETWAS SCHLECHT LAUFT!!!\n\n" + ex.Message;

            }

        }

        public void WithdrawsFromQueue()
        {
            pegbank pegpay = new pegbank();
            try
            {
                MessageQueue withdrawQueue = new MessageQueue(".\\private$\\agentWithdraw");

                withdrawQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Transaction) });

                Message msgWithdraw = withdrawQueue.Receive();

                Transaction wdraw = (Transaction)msgWithdraw.Body;

                string withdraw = pegpay.WithdrawFunds(wdraw);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
