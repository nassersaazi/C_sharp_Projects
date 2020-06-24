using PEGBANK.pegbankApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PEGBANK
{
    public partial class ViewBalances : System.Web.UI.Page
    {
        DataTable dt = new DataTable();

        pegbankApi.pegbank pegpay = new pegbankApi.pegbank();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //required to avoid the runtime error "  
            //Control 'GridView1' of type 'GridView' must be placed inside a form tag with runat=server."  
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            GridView2.DataSource = null;
            GridView2.DataBind();
            accbalanceError.Text = "";
            string AccountNumber = TextBox1.Text.ToString().Trim();
            try
            {
                if (AccountNumber.Equals(""))
                {
                    accbalanceError.Text = "Please fill in the account number!";
                    accbalanceError.Style.Add("color", "red");
                }
                else
                {
                    pegbankApi.pegbank pegpay = new pegbankApi.pegbank();
                    DataTable Details = pegpay.CheckBalance(AccountNumber);

                    if (Details.Rows.Count > 0)
                    {
                        GridView2.DataSource = Details;
                        GridView2.DataBind();
                        MultiView1.SetActiveView(View1);
                    }
                    else
                    {
                       
                        accbalanceError.Text = "Something went wrong!";
                        accbalanceError.Style.Add("color", "red");


                    }

                }
            }
            catch (Exception)
            {

                
                accbalanceError.Text = "Something went wrong!";
                accbalanceError.Style.Add("color", "red");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
        }
        

        protected void Button3_Click(object sender, EventArgs e)
        {
            
            DataTable Details = pegpay.CheckBalance("All");

            if (Details.Rows.Count > 0)
            {
                accbalanceError.Text = "";
                GridView2.DataSource = Details;
                GridView2.DataBind(); 
                MultiView1.SetActiveView(View1);
            }
            else
            {

                accbalanceError.Text = "Something went wrong!";
                accbalanceError.Style.Add("color", "red");


            }

        }

      

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
        }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Assumes the Price column is at index 2
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[2].Text = format_commas(Convert.ToInt32( e.Row.Cells[2].Text));
            }

        }

        private string format_commas(int number)
        {
            string formattedNumber = String.Format("{0:n0}", Math.Abs(number));

            return formattedNumber.ToString();
        }
    }
}