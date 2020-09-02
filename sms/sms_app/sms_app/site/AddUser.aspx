<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="AddUser.aspx.cs" Inherits="AddUser" Title="SYSTEM USER" %>
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
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> Add User
        </div>
        <div class="card-body">
            <div class="row clearfix">
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">
                <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
                   
		            <div class="modal-body">
                      First Name
                      <asp:TextBox ID="txtfname" runat="server" CssClass="form-control"></asp:TextBox>

                      Last Name
                      <asp:TextBox ID="txtlname" runat="server" CssClass="form-control"></asp:TextBox>

                      Telephone
                      <asp:TextBox ID="txtphone" runat="server" CssClass="form-control"></asp:TextBox>

                      Email
                      <asp:TextBox ID="txtemail" runat="server" CssClass="form-control"></asp:TextBox>

                      Vendor
                      <asp:DropDownList ID="ddlAreas" runat="server" OnDataBound="ddlAreas_DataBound" CssClass="form-control">
                      </asp:DropDownList>

                      User Role
                      <asp:DropDownList ID="ddlUserType" runat="server" OnDataBound="ddlUserType_DataBound" CssClass="form-control">
                      </asp:DropDownList>

                      <br/><asp:CheckBox ID="chkActive" runat="server" Text=" Is Active" Checked="true" /><br/>

                      <asp:MultiView ID="MultiView2" runat="server">
                        <asp:View ID="View3" runat="server">
                                   
                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" Visible="false"></asp:TextBox></td>
                                           
                        </asp:View>
                        <asp:View ID="View2" runat="server">
                         <asp:CheckBox ID="CheckBox1" runat="server" Text=" Tick To Reset Password" /> <br/>
                        </asp:View>
                    </asp:MultiView>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" Text="SUBMIT DETAILS"  OnClick="btnOK_Click" />
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
            </div>
        </div>
    </div>
</div>
    <asp:Label ID="lblCode" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="lblUsername" runat="server" Text="0" Visible="False"></asp:Label><br />
    &nbsp;

</asp:Content>





