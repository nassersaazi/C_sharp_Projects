using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PEGBANK
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDeposit_Click(object sender, EventArgs e)
        {
            Response.Redirect("Deposit.aspx");
        }

        protected void btnWithdraw_Click(object sender, EventArgs e)
        {
            Response.Redirect("Withdraw.aspx");
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("RegisterUser.aspx");
        }

        protected void btnCheckBalance_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewBalances.aspx");
        }
    }
}