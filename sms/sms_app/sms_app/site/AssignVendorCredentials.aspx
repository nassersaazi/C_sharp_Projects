<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="AssignVendorCredentials.aspx.cs" Inherits="AssignVendorCredentials" Title="Assign Vendor Credentials" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%--<%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>--%>
 <asp:ScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" />

<div class="col-lg-12">
   
    <div class="card mb-3">
        <div class="card-header">
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> Assign Vendor Credentilas
        
        </div>
        <div class="card-body">
        <div class="row clearfix" >
                <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">
                   
                            <div class="col-md-3">
                            Vendor
                           <asp:DropDownList ID="ddlVendors" runat="server" AutoPostBack="True"  CssClass="form-control"  OnDataBound="ddlAreas_DataBound" OnSelectedIndexChanged="ddlAreas_SelectedIndexChanged" >
                           </asp:DropDownList>
                           </div>
                           <asp:MultiView ID="MultiView3" runat="server" ActiveViewIndex="0">
                                <asp:View ID="View3" runat="server">
                                   <div class="col-md-3">
                                        Secrete Key
                                        <asp:TextBox ID="txtKey" runat="server" CssClass="form-control"></asp:TextBox>
		                            </div>
                                </asp:View>

                                <asp:View ID="View4" runat="server">
                                     <div class="col-md-3">
                                        Upload certificate
                                        <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
		                            </div>
                                    <div class="col-md-2">
                                        certificate Password
                                        <asp:TextBox ID="txtCertPassword" runat="server" CssClass="form-control"></asp:TextBox>
		                            </div>
                                </asp:View>
                            </asp:MultiView>
                            <div class="col-md-3">
                            Vendor Password
                            <asp:TextBox ID="txtVendorPassword" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                            Save<br/>
                            <asp:Button ID="btnAssignKey" runat="server" CssClass="btn btn-success" Text="Save" OnClick="Button1_Click" />
                            </div>

                            <hr/>
                    
                    <div class="col-md-12" style="overflow-x:auto; margin-top:20px;">
                    <div class="row clearfix">
                        <div class="col-md-4">
                             Enter (Vendor or Mask)
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            Search..<br/>
                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="Button1_Ok" />
                        </div>
                    </div>
                    <asp:MultiView ID="MultiView2" runat="server">
                        <asp:View ID="View2" runat="server">
                        <asp:DataGrid ID="DataGrid1" runat="server"  AutoGenerateColumns="True" style="margin-top:10px;"
                                UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                                    HorizontalAlign="Justify" AllowPaging="True" PageSize="20" OnItemCommand="DataGrid1_ItemCommand"
                                OnPageIndexChanged="DataGrid1_PageIndexChanged">
                                <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                                <PagerStyle  ForeColor="#4380B8" HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                <Columns>
                                   <asp:ButtonColumn CommandName="btnEdit" HeaderText="Edit" Text="Area" DataTextField="VendorCode">
                                        
                                    </asp:ButtonColumn>
                                                   
                                </Columns>
                                <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" Font-Names="Courier New" />
                            </asp:DataGrid>
                            </asp:View>
                        </asp:MultiView>
                    </div>
                </asp:View>
            </asp:MultiView>
            </div>
        </div>
    </div>
</div>


</asp:Content>
