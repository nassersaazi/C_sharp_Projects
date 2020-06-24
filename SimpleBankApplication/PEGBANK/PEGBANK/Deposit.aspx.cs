using PEGBANK.pegbankApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PEGBANK
{
    public partial class Deposit : System.Web.UI.Page
    {
        pegbankApi.pegbank pegpay = new pegbankApi.pegbank();
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string AccountNumber = TextBox1.Text.ToString();
                string Amount = txtAmount.Text.ToString();
                if (String.IsNullOrEmpty(AccountNumber) | String.IsNullOrEmpty(Amount))
                {
                    Response.Write("Please fill in all fields");
                 
                }
                else
                {
                    Transaction req = new Transaction();
                    req.AccountNumber = AccountNumber;
                    req.Amount = Int32.Parse(Amount);
                    RequestFloatsToQueue(req);
                   
                    depositError.Text = "Deposited successfully!";
                    depositError.Style.Add("color", "green");
                       
                }
                
            }
            catch (Exception)
            {

               // Response.Write("An error occurred!");
                depositError.Text = "An error occurred!";
                depositError.Style.Add("color", "red");
            }
        }

        private void RequestFloatsToQueue(Transaction fr)
        {
            Transaction agentRequest = new Transaction();
            agentRequest.AccountNumber = fr.AccountNumber;
            agentRequest.Amount = fr.Amount;

            MessageQueue msQueue;
            if (!MessageQueue.Exists(".\\private$\\agentDeposit"))
            {
                msQueue = MessageQueue.Create(".\\private$\\agentDeposit");
                msQueue.Send(agentRequest);
            }
            else
            {
                msQueue = new MessageQueue(".\\private$\\agentDeposit");
                msQueue.Send(agentRequest);
            }
        }
    }
}