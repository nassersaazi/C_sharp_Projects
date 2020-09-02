<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="Vendors, App_Web_exnx0sam" title="Add vendor" %>
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
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> Add vendors
        </div>
        <div class="card-body">
            <div class="row clearfix">

             <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">

                    <div class="modal-content col-md-6  col-sm6 col-xs-10"  style="margin:0 auto;">
                        
		                <div class="modal-body">
                             Vendor Type
                            <asp:DropDownList ID="ddlUsers" runat="server"  CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Value="0" Text="select Vendor Type"></asp:ListItem>
                                <asp:ListItem Value="PREPAID" Text="PREPAID"></asp:ListItem>
                                <asp:ListItem Value="POSTPAID" Text="POSTPAID"></asp:ListItem>
                            </asp:DropDownList>


                            Vendor Name
                            <asp:TextBox ID="txtVendorName" runat="server" CssClass="form-control"></asp:TextBox>
		                    		                    
                            Vendor Contact (if any)
                            <asp:TextBox ID="txtVendorContact" runat="server"  CssClass="form-control"></asp:TextBox>

                             Vendor Email (if any)
                            <asp:TextBox ID="txtEmail" runat="server"  CssClass="form-control"></asp:TextBox>

                            Vendor Code
                            <asp:TextBox ID="txtVendorCode" runat="server" CssClass="form-control"></asp:TextBox>

                            Vendor Password
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                            

                            Vendor Secret Key
                            <asp:TextBox ID="txtKey" runat="server" CssClass="form-control"></asp:TextBox>
                            
                           
                             
                              <br/>
                            <asp:CheckBox ID="IsActive" runat="server" Text=" Is Active" ></asp:CheckBox> &nbsp;&nbsp;&nbsp;&nbsp;

                          <%--  <asp:CheckBox ID="IsPrepaid" runat="server" Text=" Is Prepaid" ></asp:CheckBox> &nbsp;&nbsp;&nbsp;&nbsp;--%>

                        <asp:Button ID="Button1" runat="server" CssClass="btn btn-success pull-right" Text="Save Vendor" OnClick="Button1_Click" />

                        </div>
                </div>
                </asp:View>
       
    </asp:MultiView>
            </div>
        </div>
    </div>
</div>
    <%--<asp:Label ID="lblCode" runat="server" Text="0" Visible="False"></asp:Label><br />--%>
</asp:Content>

