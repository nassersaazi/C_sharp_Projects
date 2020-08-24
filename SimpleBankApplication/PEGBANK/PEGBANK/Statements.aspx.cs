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
    public partial class Statements : System.Web.UI.Page
    {
        

        pegbankApi.pegbank pegpay = new pegbankApi.pegbank();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //required to avoid the runtime error "  
            //Control 'GridView1' of type 'GridView' must be placed inside a form tag with runat=server."  
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            GridView1.DataSource = null;
            GridView1.DataBind();
            statementError.Text = "";
            string AccountNumber = TextBox2.Text.ToString();
            string Start = DropDownList2.Text.ToString();
            string End = DropDownList3.Text.ToString();

            if (String.IsNullOrEmpty(Start) | String.IsNullOrEmpty(End))
            {
                statementError.Text = "Please fill in the dates!";
                statementError.Style.Add("color", "red");
            }
            else
            {

                DataTable dt = pegpay.GetStatement(AccountNumber, DateTime.Parse(Start), DateTime.Parse(End));

                if (dt.Rows.Count > 0)
                {
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                    MultiView2.SetActiveView(View2);
                }
                else
                {
                    //Response.Write("Something went wrong!!");
                    statementError.Text = "Currently no transactions for this account!";
                    statementError.Style.Add("color", "red");
                }

            }

        }

        protected void excel_Click(object sender, EventArgs e)
        {
            ExportGridToExcel();

        }
        private void ExportGridToExcel()
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string FileName = "Statement" + DateTime.Now + ".xls";
            StringWriter strwritter = new StringWriter();
            HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            GridView1.GridLines = GridLines.Both;
            GridView1.HeaderStyle.Font.Bold = true;
            GridView1.RenderControl(htmltextwrtter);
            Response.Write(strwritter.ToString());
            Response.End();

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Assumes the Price column is at index 1 and 4
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[1].Text = format_commas(Convert.ToInt32(e.Row.Cells[1].Text));
                e.Row.Cells[4].Text = format_commas(Convert.ToInt32(e.Row.Cells[4].Text));
            }
        }

        private string format_commas(int number)
        {
            string formattedNumber = String.Format("{0:n0}", Math.Abs(number));

            return formattedNumber.ToString();
        }
    }
}