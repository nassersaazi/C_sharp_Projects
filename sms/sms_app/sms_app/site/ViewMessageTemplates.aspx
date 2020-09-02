<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="ViewMessageTemplates.aspx.cs" Inherits="ViewMessageTemplates" Title="MESSAGE TEMPLATES" %>
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
        <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i> View Message Templates
        </div>
        <div class="card-body">
            <div class="row clearfix">
             <asp:MultiView ID="MultiView2" runat="server">
               
                <asp:View ID="View2" runat="server">
                    <div class="col-md-4">
                        <label>Template Title</label>
                        <asp:TextBox ID="txtTitle" runat="server"  CssClass="form-control">
                        </asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label>Created By</label>
                        <asp:TextBox ID="txtUser" runat="server"  CssClass="form-control">
                        </asp:TextBox>
                    </div>

                    
                    <div class="col-md-4">
                        <label>Search..</label><br/>
                         <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search" />
                    </div>

                    <div class="col-md-12"><br /></div>
                    
                     <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="true"
                    OnItemCommand="DataGrid1_ItemCommand" 
                    UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover"
                    OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="30" ShowFooter="True">
                    <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8" HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <Columns>
                                    <asp:ButtonColumn HeaderStyle-CssClass="" CommandName="btnEdit" HeaderText="Edit" Text="Edit" DataTextField="" Visible="false">
                                        <ItemStyle CssClass="btn btn-danger btn-sm" ForeColor="#FEFEFE"/>
                                    </asp:ButtonColumn>
                                    
                                </Columns>

                    <HeaderStyle BackColor="#006699"  ForeColor="White"  />
                </asp:DataGrid>

                                   
                </asp:View>
            </asp:MultiView>
            </div>
        </div>
    </div>
</div>

    <asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />

</asp:Content>



