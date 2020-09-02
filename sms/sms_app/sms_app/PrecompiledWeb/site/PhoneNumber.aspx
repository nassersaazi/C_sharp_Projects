<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="PhoneNumber, App_Web_exnx0sam" title="ADD PHONE NUMBER(S)" %>
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
        <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i> Phone Number Panel
        
        </div>
        <div class="card-body">
             <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">
             <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
                <div class="modal-header">
			        <h4>Add Numbers to Group </h4>
		        </div>
		        <div class="modal-body">
                Group Name
                <asp:DropDownList ID="ddllists" CssClass="form-control" runat="server" OnDataBound="ddllists_DataBound">
                </asp:DropDownList>
                <br/>
                Receipient Type
                <asp:DropDownList ID="ddlReceipient" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlReceipient_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Value="1">Add from File</asp:ListItem>
                    <asp:ListItem Value="0">Input Contact</asp:ListItem>
                    
                </asp:DropDownList>
                <br/>
                   <asp:MultiView ID="MultiView2" runat="server">
                        <asp:View ID="View3" runat="server">
                     Enter Phone Number
                     <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" MaxLength="12"></asp:TextBox>

                    Enter Name(s)
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox><br/>
                </asp:View>
                
                <asp:View ID="View4" runat="server">
                    Browse File <a  href="SMS_List_Sample.csv" class="pull-right"> <i class='fa fa-file-excel-o pull-right'></i> Template</a>
                    <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                    
                </asp:View>
             </asp:MultiView>
             </div>
                <div class="modal-footer">
                 
                <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" Text="Upload Number(s)" OnClick="Button1_Click" />
             </div>
            
             </asp:View>
              </div>
            <asp:View ID="View2" runat="server">
                <div class=" modal-content col-md-6  col-sm-6  col-xs-10" style="margin:0 auto;">
		
		        <div class="modal-body">
			        <asp:Label ID="lblQn" runat="server" Font-Bold="True" ForeColor="Maroon" Text="."></asp:Label>               
		        </div>
		        <div class="modal-footer">
                    <asp:Button ID="btnYes" runat="server" CssClass="btn btn-success pull-right" OnClick="btnYes_Click" Text="Yes" />
                    <asp:Button ID="btnNo" runat="server" CssClass="btn btn-danger pull-left" OnClick="btnNo_Click" Text="No" />
		        </div>
	        </div>
            </asp:View>
        </asp:MultiView>
        </div>
</div>

    <ajaxToolkit:FilteredTextBoxExtender id="FilteredTextBoxExtender1" runat="server"
        TargetControlID="txtPhoneNumber" ValidChars="0123456789">
    </ajaxToolkit:FilteredTextBoxExtender>
    <asp:Label ID="lblPath" runat="server" Text="." Visible="False"></asp:Label>
</asp:Content>

