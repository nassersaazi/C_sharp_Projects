<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="CreditUser, App_Web_dqet1j14" title="Credit Vendor" %>

<%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>
 <%@ Import
  Namespace="System.Threading" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


    <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" />

<div class="col-lg-12">
    <div class="card mb-3">
        <div class="card-header">
        <i class="fa fa fa-brifcase"></i> Accounts <i class='fa fa-arrow-right'></i> Credit Vendor
        </div>
        <div class="card-body">
            <div class="row clearfix">
             <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="View1" runat="server">
                   <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
                            <div class="modal-header">
			                    <center>Credit User</center>
		                    </div>
		                    <div class="modal-body">
                            Select User
                             <br/>
                            <asp:DropDownList ID="ddlUsers" runat="server" OnSelectedIndexChanged="ddlusers_SelectedIndexChanged" CssClass="form-control" AutoPostBack="true" AppendDataBoundItems="true">
                                
                            </asp:DropDownList><br/>
		        

                            Current Balance
                            <asp:TextBox ID="txtBal" runat="server" CssClass="form-control" Enabled="False"></asp:TextBox><br/>

                            Credit
                            <asp:TextBox ID="txtCredit" runat="server" CssClass="form-control" type="Number" onkeypress="return functionx(event)"></asp:TextBox>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" OnClick="Button1_Click" Text="Credit Vendor" />
                         </div>
                    </div>
                 </asp:View>

            </asp:MultiView>
            </div>
        </div>
    </div>
</div>

<script type = "text/javascript">
    function functionx(evt) {
        if (evt.charCode > 31 && (evt.charCode < 48 || evt.charCode > 57)) {
            //alert("Allow Only Numbers");
            return false;
        }
    }
     </script>

</asp:Content>
