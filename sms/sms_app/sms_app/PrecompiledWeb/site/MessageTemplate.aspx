<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="MessageTemplate, App_Web_exnx0sam" title="SMS SENDING PANEL" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%--<%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>--%>
 <asp:ScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" 
        />
<div class="col-lg-12">
   
    <div class="card mb-3">
        <div class="card-header">
        <i class="fa fa fa-envelope"></i> OUTBAOUND SMS <i class='fa fa-arrow-right'></i> Message Template
        </div>
        <div class="card-body row clearfix ">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
    <asp:View ID="View1" runat="server">
        
            <div class="modal-content col-md-7  col-sm-7 col-xs-7">
				 
                <div class="modal-body">                                       
                       
					<label>Template Title</label>
                        <div class="form-group input-group">							
							<asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>
						</div> 
                    <asp:TextBox ID="txtId" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    <label>Variables</label>
                        <div class="form-group input-group">							
							<asp:FileUpload ID="ddlvariable" runat="server" CssClass="form-control" />
						</div> 
               

					<label> Message Body</label>
					<div class="form-group input-group">
						<asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" Height="100" TextMode="MultiLine" onKeyDown="textCounter(document.txtMessage,document.txtCount,125)"
                                onKeyUp="textCounter(document.txtMessage,document.txtCount,125)"></asp:TextBox>
                    </div>
                    
					<asp:Button ID="btnOK" runat="server" CssClass="btn btn-success pull-right" OnClick="btnOK_Click" Text="SEND MESSAGE" />
				</div>
				<div class="modal-footer">
					
				</div>
			</div>

            <div class="modal-content col-md-4  col-sm-4 col-xs-4 " style="margin-left:40px;">
                <div class="modal-header">
                   Rules  
                    <a  href="Template_sms.csv" class="pull-right"> <i class='fa fa-file-excel-o'></i> Template</a>
                    
                </div>
                <div class="modal-body"> 
                <ol>
                    <li>The sender is expected to use a <b>@</b> sign on every dynamic variable in the message to be sent.</li>
                    <li>All defined variables must be defined in the CSV file template for a message to be valid.</li>
                    <li>The name of the defined variables must mach the column names in the attached CSV file</li>
                    <li>You can define as many variables as you can for as long as they are respectively defined in the file</li>
                </ol>
                    The message sample can be like:<br/>
                 </div>
                    <div class="modal-body" style="border: 1px solid red;"> 
                        Hello <b>@Name</b>,<br/>
                        Your account <b>@Account</b> has an outstanding balance of <b>@Balance</b> and it will expire on <b>@Expdate</b><br/>
                        Thank you....<br/>
                    </div>
               
                
            </div>
        
    </asp:View>
    </asp:MultiView>
    </div>
</div>
</div>
<script type="text/javascript">
    function count(clientId) {
        var txtMessage = document.getElementById(clientId);
        var spanDisplay = document.getElementById('spanDisplay');
        spanDisplay.innerHTML = txtMessage.value.length;
    }
</script>
</asp:Content>

