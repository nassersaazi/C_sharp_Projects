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
    public partial class Withdraw : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string AccountNumber = TextBox1.Text.ToString();
                string Amount = txtAmount.Text.ToString();

                if (String.IsNullOrEmpty(Amount))
                {
                    Response.Write("Please fill in all fields!!"); ;
                }
                else
                {
                    Transaction trans = new Transaction();
                    trans.AccountNumber = AccountNumber;
                    trans.Amount = Int32.Parse(Amount);
                    RequestFloatsToQueue(trans);
                    
                    withrawError.Text = "Withdraw successful!";
                    withrawError.Style.Add("color", "green");
                       
                }

                }catch(Exception){
                //Response.Write("An error occuredd");
                withrawError.Text = "An error occuredd!";
                withrawError.Style.Add("color", "red");
            }
            }

        private void RequestFloatsToQueue(Transaction fr)
        {
            Transaction agentRequest = new Transaction();
            agentRequest.AccountNumber = fr.AccountNumber;
            agentRequest.Amount = fr.Amount;

            MessageQueue msQueue;
            if (!MessageQueue.Exists(".\\private$\\agentWithdraw"))
            {
                msQueue = MessageQueue.Create(".\\private$\\agentWithdraw");
                msQueue.Send(agentRequest);
            }
            else
            {
                msQueue = new MessageQueue(".\\private$\\agentWithdraw");
                msQueue.Send(agentRequest);
            }
        }

    }
    }
