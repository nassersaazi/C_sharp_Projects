<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="Password.aspx.cs" Inherits="Password" Title="RESET PASSWORD" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>
 <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" />
   
<div class="col-lg-12">
    <div class="card mb-3">
        <div class="card-header">
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> Change Password
        </div>
        <div class="card-body">
            <div class="row clearfix">
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">
                <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
                            <div class="modal-header">
			                    <center><h4>Change Password</h4></center>
		                    </div>
		                    <div class="modal-body">
                             Old Password
                             <asp:TextBox ID="txtOldPasswd" runat="server" MaxLength="12" CssClass="form-control" TextMode="Password"></asp:TextBox>
		        

                            New Password
                            <asp:TextBox ID="txtNewPasswd" runat="server" CssClass="form-control" MaxLength="12" TextMode="Password"></asp:TextBox>

                            Confirm Password
                            <asp:TextBox ID="txtConfirm" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                            </div>
                            <div class="modal-footer">
          
                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" Text="Change Password" OnClick="Button1_Click" />
                         </div>
                    </div>
                </asp:View>
            </asp:MultiView>
            </div>
        </div>
    </div>
</div>


    <asp:Label ID="lblPath" runat="server" Text="." Visible="False"></asp:Label>
</asp:Content>

