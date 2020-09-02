<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="ViewListDetails, App_Web_exnx0sam" title="LIST PHONE NUMBERS" %>
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
        <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i> Contact Groups
        </div>
        <div class="card-body">
            <div class="row clearfix">
             <asp:MultiView ID="MultiView2" runat="server">
                <asp:View ID="View2" runat="server">
                    <div class="col-md-2">
                        <label>Contact group</label>
                        <asp:DropDownList ID="ddllists" runat="server"  CssClass="form-control" OnDataBound="ddllists_DataBound">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <label>Phone number</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="12"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label>enter name</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label>Search..</label><br/>
                         <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search" />
                    </div>
                   
                     <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    OnItemCommand="DataGrid1_ItemCommand" style="white-space:nowrap; margin-top:10px;"
                    UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover"
                    OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="30" ShowFooter="True">
                    <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <Columns>
                        <asp:BoundColumn DataField="RecordID" HeaderText="Code" Visible="False"></asp:BoundColumn>
                        <asp:ButtonColumn CommandName="btnChange" HeaderText="On/Off" Text="PhoneNumber" DataTextField="PhoneNumber" Visible="False">
                            
                        </asp:ButtonColumn>
                        <asp:BoundColumn DataField="No." HeaderText="No.">
                            
                        </asp:BoundColumn>
                        <asp:ButtonColumn DataTextField="PhoneNumber" HeaderText="Edit" Text="PhoneNumber" CommandName="btnEdit">
                           
                        </asp:ButtonColumn>
                        <asp:BoundColumn HeaderText="Phone" Visible="False" DataField="PhoneNumber"></asp:BoundColumn>
                        <asp:BoundColumn DataField="PhoneName" HeaderText="Phone Name">
                           
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Active" HeaderText="Active">
                           
                        
                           
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date">
                          
                        </asp:BoundColumn>
                    </Columns>
                    <HeaderStyle BackColor="#4380B8" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
                </asp:DataGrid>

                </asp:View>

                <asp:View ID="View1" runat="server">
                    <div class=" modal-content col-md-6  col-sm-6  col-xs-10" style="margin:0 auto;">
		                <div class="modal-header">
			                <center>EDIT PHONE NUMBER</center>
		                </div>
		                <div class="modal-body">
			                Phone Number
                             <asp:TextBox ID="txtPhoneNumber" runat="server" MaxLength="12" CssClass="form-control"></asp:TextBox>

                            Name(s)
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>

                            <asp:CheckBox ID="chkActive" runat="server" Text=" Is Active on List" /><br/>
                                   
		                </div>
		                <div class="modal-footer">
                             <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" OnClick="Button1_Click" Text="Update Number" />
		                </div>
	                </div>
                </asp:View>
            </asp:MultiView>
            </div>
        </div>
    </div>
</div>

    <asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />
    <ajaxToolkit:FilteredTextBoxExtender id="FilteredTextBoxExtender1" runat="server"
        TargetControlID="txtPhone" ValidChars="0123456789">
    </ajaxToolkit:FilteredTextBoxExtender><ajaxToolkit:FilteredTextBoxExtender id="FilteredTextBoxExtender2" runat="server"
        TargetControlID="txtPhoneNumber" ValidChars="0123456789">
    </ajaxToolkit:FilteredTextBoxExtender>
</asp:Content>



