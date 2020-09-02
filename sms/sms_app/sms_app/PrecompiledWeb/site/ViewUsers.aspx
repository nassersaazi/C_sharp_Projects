<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="ViewUsers, App_Web_exnx0sam" title="VIEW USERS" %>
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
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> View Users
        </div>
        <div class="card-body">
            <div class="row clearfix" style="overflow-x:auto;">
            <asp:MultiView ID="MultiView2" runat="server">
                <asp:View ID="View2" runat="server">
                <div class="col-md-2">
                    <label>Vendor</label>
                    <asp:DropDownList ID="ddlAreas" runat="server" AutoPostBack="True" CssClass="form-control" OnDataBound="ddlAreas_DataBound">
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label>User Role</label>
                    <asp:DropDownList ID="ddlUserType" runat="server"  CssClass="form-control">
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label>name(s)</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <label>Search..</label><br/>
                    <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search" />
                </div>


                <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                    UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                    OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True" style="white-space:nowrap; margin-top: 10px;">
                   <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <Columns>

                        <asp:BoundColumn DataField="UserId" HeaderText="Code" Visible="False"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Username" HeaderText="Username" Visible="False"></asp:BoundColumn>
                      
                        <asp:ButtonColumn CommandName="btnCredit" HeaderText="Credit" Text="Username" DataTextField="Username" Visible="false">
                            <HeaderStyle Width="15%" />
                            <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" ForeColor="Blue" />
                        </asp:ButtonColumn>


                        <asp:ButtonColumn DataTextField="Username" HeaderText="Edit" Text="Username" CommandName="btnEdit">
                            <HeaderStyle Width="15%" />
                            <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" ForeColor="Blue" />
                        </asp:ButtonColumn>
                        <asp:BoundColumn DataField="fullName" HeaderText="Name">
                            <HeaderStyle Width="25%" />
                            <ItemStyle Width="120px" />
                        </asp:BoundColumn>
                        <asp:BoundColumn HeaderText="Phone" Visible="False">
                            <HeaderStyle Width="10%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="UserRole" HeaderText="Role">
                            <HeaderStyle Width="10%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="VendorCode" HeaderText="Vendor">
                            <HeaderStyle Width="10%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Active" HeaderText="Active">
                            <HeaderStyle Width="10%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Balance" HeaderText="SMS Balance">
                            <HeaderStyle Width="10%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="CreationDate" HeaderText="Date" Visible="False">
                            <HeaderStyle Width="10%" />
                        </asp:BoundColumn>
                    </Columns>
                    
                </asp:DataGrid>

                </asp:View>

                <asp:View ID="View1" runat="server">
                    <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
                            <div class="modal-header">
			                    <center>Credit User</center>
		                    </div>
		                    <div class="modal-body">
                             <asp:Label ID="lblCredit" runat="server" Font-Bold="True" Font-Names="Arial" ForeColor="Blue" Text="."></asp:Label>
                             <br/>
                             Username
                             <asp:TextBox ID="txtUserName" runat="server" MaxLength="12" CssClass="form-control" Enabled="False"></asp:TextBox>
		        

                            Name(s)
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Enabled="False"></asp:TextBox>

                            Credit
                            <asp:TextBox ID="txtCredit" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" OnClick="Button1_Click" Text="Add Credit" />
                         </div>
                    </div>
                 </asp:View>

            </asp:MultiView>
            </div>
        </div>
      </div>
    </div> 
     

                        
    <asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />
    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
        TargetControlID="txtCredit" ValidChars="0123456789">
    </ajaxToolkit:FilteredTextBoxExtender>
</asp:Content>



