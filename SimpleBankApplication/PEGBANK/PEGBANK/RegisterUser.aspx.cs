
using PEGBANK.pegbankApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PEGBANK
{
    public partial class RegisterUser : System.Web.UI.Page
    {
        pegbankApi.pegbank pegpay = new pegbankApi.pegbank();
        PostTransactionRequest req = new PostTransactionRequest();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            try
            {
                
                string firstName = txtName.Text.ToString().Trim();
                string lastName = TextBox1.Text.ToString().Trim();
                string phone = TextBox2.Text.ToString().Trim();
                string Email = txtEmail.Text.ToString();
                
                req.firstName = firstName;
                req.lastName = lastName;
                req.phoneNumber = phone;
                req.email = Email;

                
                if (String.IsNullOrEmpty(firstName) | String.IsNullOrEmpty(Email))
                {
                    
                    registerError.Text = "Please fill in all fields!";
                    registerError.Style.Add("color", "red");
                }
                else
                {
                    registerError.Text = "";
                    string result = pegpay.RegisterClient(req);
                    registerError.Text = "Registration " + result; 
                    registerError.Style.Add("color", "green");
                }
            }
            catch (Exception )
            {
                registerError.Text = "An error occurred!";
                registerError.Style.Add("color", "red");
            }

        }
    }
}