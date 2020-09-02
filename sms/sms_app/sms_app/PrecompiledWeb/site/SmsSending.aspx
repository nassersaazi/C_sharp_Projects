<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="SmsSending, App_Web_dqet1j14" title="SMS SENDING PANEL" %>
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
        <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i> Send SMS 
       <%-- <a  href="Send_SMS_Template.csv" class="pull-right"> <i class='fa fa-file-excel-o'> </i> Send to List Template </a> 
         <a  href="Template_sms.csv" class="pull-right"> <i class='fa fa-file-excel-o'> </i> Send as SMS Template </a>--%>
        </div>
        <div class="card-body row clearfix ">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
    <asp:View ID="View1" runat="server">
        
            <div class="modal-content col-md-7  col-sm-7 col-xs-7">
				 
                <div class="modal-body">
                            <label>Select Mask</label>
							<div class="form-group input-group">
								
								<asp:DropDownList ID="ddlMasks" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                                   
                                </asp:DropDownList>
                            </div>
                        <label>Select Receipient</label>
							<div class="form-group input-group">
								
								<asp:DropDownList ID="ddlReceipient" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlReceipient_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                    <asp:ListItem Value="0">Send To Number(s)</asp:ListItem>
                                    <asp:ListItem Value="1">Send To Contact Group</asp:ListItem>
                                    <asp:ListItem Value="2">Upload contact File</asp:ListItem>
                                    <asp:ListItem Value="3">Send To SMS Template</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        <asp:MultiView ID="MultiView2" runat="server">
                        <asp:View ID="View4" runat="server">
						    <label>Enter Number(s)</label>
                                <div class="form-group input-group">
								
								    <asp:TextBox ID="txtPhones" runat="server" CssClass="form-control"></asp:TextBox>
							    </div> 
                        </asp:View>
                        <asp:View ID="View5" runat="server">
							<label>Contact group</label>
							<div class="form-group input-group">
								
								<asp:DropDownList ID="ddlists" runat="server" CssClass="form-control" OnDataBound="ddlists_DataBound">
                                </asp:DropDownList>
                            </div> 
						</asp:View>
                        <asp:View ID="View6" runat="server">
                            Upload Contact file <a  href="Send_SMS_Template.csv" class="pull-right"> <i class='fa fa-file-excel-o'></i> Contacts Template </a> 
							<div class="form-group input-group">
								
								<asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                            </div> 
                        </asp:View>

                        <asp:View ID="View7" runat="server">
                            Upload Template file <a  href="Template_sms.csv" class="pull-right"> <i class='fa fa-file-excel-o'></i> SMS Template </a>
							<div class="form-group input-group">
								
								<asp:FileUpload ID="ddlvariable" runat="server" CssClass="form-control" />
                            </div> 
                        </asp:View>
                </asp:MultiView>


                           <%-- <label>Message Template</label>
							<div class="form-group input-group">
								
								<asp:DropDownList ID="ddlMessageTemplates" runat="server" CssClass="form-control"
                                 OnSelectedIndexChanged="ddlMessageTemplates_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                    <asp:ListItem Value="">Select Message Template</asp:ListItem>
                                </asp:DropDownList>
                            </div>--%>


							<label> Message To Send</label>
							<div class="form-group">
								<asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" Height="100" TextMode="MultiLine" onKeyDown="textCounter(document.txtMessage,document.txtCount,125)"
                                onKeyUp="textCounter(document.txtMessage,document.txtCount,125)"></asp:TextBox>
                            </div>
                          
                            <br/><asp:CheckBox ID="chkSchedule" runat="server" Text=" Schedule SMS" AutoPostBack="true" OnCheckedChanged="ChckedChanged" /><br/>

                           </div>

                            <asp:MultiView ID="MultiView3" runat="server">
                                <asp:View ID="View8" runat="server">
                                <div class="modal-body row clearfix ">
                                    <div class="col-md-5">
                                     Scheduled Date<br/>
                                    <asp:TextBox ID="txtSendDate" runat="server" CssClass="input-group form-control datepicker"></asp:TextBox>
                                    </div>

                                    <div class="col-md-5 ">
                                     Scheduled Time<br/>
                                    <asp:TextBox ID="txtSendTime" runat="server" Type="Time" CssClass="input-group form-control "></asp:TextBox>
                                    </div>
                                </div>
                                </asp:View>
                            </asp:MultiView>
							<div class="modal-footer">
					
					            <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success pull-right" OnClick="btnOK_Click" Text="SEND MESSAGE" />
					        </div>
				 <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
                                 TargetControlID="txtPhones" ValidChars="0123456789,+" Enabled="True">
                 </ajaxToolkit:FilteredTextBoxExtender>
			</div>

                <div class="modal-content col-md-4  col-sm-4 col-xs-4 " style="margin-left:40px;">
                <div class="modal-header">
                 <h5>Sending SMS  Guidelines   </h5>
                </div>
                <asp:MultiView ID="MultiView4" runat="server">

                <asp:View ID="View9" runat="server">
                    <div class="modal-body">
                        Send an SMS to a contact that starts with either <b>256</b> or <b>+256</b> or <b>07..</b>
                    </div>
                 </asp:View>

                  <asp:View ID="View10" runat="server">
                    <div class="modal-body">
                        Sending an SMS to contact group involves the following:
                        <ol>
                            <li>Ensure that the contact group created has targeted contacts</li>
                            <li>Contact groups have pre defined numbers on which messages are sent</li>
                            <li>Disabled Numbers in a contact group do not receive messages</li>
                        </ol>
                    </div>
                 </asp:View>
                  <asp:View ID="View11" runat="server">
                    <div class="modal-body">
                        Sending Messages to uploaded contacts involes:
                        <ol>
                            <li>Upload a contact file with contacts receipients</li>
                            <li>A file contains of only numbers intended to receive SMS</li>
                            <li>Invalid Phone number will be flagged out and not processed</li>
                        </ol>
                        <a  href="Send_SMS_Template.csv" class="pull-right"> <i class='fa fa-file-excel-o'></i> Contacts Template </a>
                    </div>
                 </asp:View>
                <asp:View ID="View12" runat="server">
                <div class="modal-body"> 
                <ol>
                    <li>The sender is expected to use a <b><%=variableChar%></b> sign on every dynamic variable in the message to be sent.</li>
                    <li>All defined variables must be defined in the CSV file template for a message to be valid.</li>
                    <li>The name of the defined variables must mach the column names in the attached CSV file</li>
                    <li>You can define as many variables as you can for as long as they are respectively defined in the file</li>
                </ol>
                <a  href="Template_sms.csv" class="pull-right"> <i class='fa fa-file-excel-o'></i> SMS Template </a>
                    The message sample can be like:<br/>
                 </div>
                <div class="modal-body" style="border: 1px solid red;"> 
                    Hello <b><%=variableChar%>Name</b>,<br/>
                    Your account <b><%=variableChar%>Account</b> has an outstanding balance of <b><%=variableChar%>Balance</b> and it will expire on <b><%=variableChar%>Expdate</b><br/>
                    Thank you....<br/>
                </div>
               
                 </asp:View>
            </asp:MultiView>
            </div>
        
    </asp:View>
     <asp:View ID="View2" runat="server">
     <div class=" modal-content col-md-6  col-sm-6  col-xs-10" style="margin:0 auto;">
		<div class="modal-header">
			<center><h4>SMS CREDIT MESSAGE</h4></center>
		</div>
		<div class="modal-body">
			<asp:Label ID="lblerror" runat="server" Font-Bold="True" ForeColor="#C00000" Text="."></asp:Label>
                                   
		</div>
		<div class="modal-footer">

		</div>
	</div>
      </asp:View>

      <asp:View ID="View3" runat="server">

      <div class=" modal-content col-md-6  col-sm-6 col-xs-10" style="margin:0 auto;">
		<div class="modal-header">
			<center><h4>SMS CONFIRMATION</h4></center>
		</div>
		<div class="modal-body">
            <div>
                <asp:Label ID="Label1" runat="server" Text="."></asp:Label> 
            </div>
			<div><label>Receipient</label></div>
        <div class="form-group input-group">
			<asp:TextBox ID="txtviewlistname" runat="server" CssClass="form-control" Enabled="False"></asp:TextBox>
		</div> 
        <span style="display:none">
		    <label>Receipient</label>
		    <div class="form-group input-group">
			    <asp:TextBox ID="txtviewprefix" runat="server" CssClass="form-control" Enabled="False"></asp:TextBox>
            </div>
        </span> 
		<label>Message To Send</label>
		<div class="form-group input-group">
			<asp:TextBox ID="txtViewMessage" runat="server" CssClass="form-control" Height="100" TextMode="MultiLine"  Enabled="False" ReadOnly="True"></asp:TextBox>
        </div>
      
		</div>
		<div class="modal-footer">
        <asp:Button ID="Button2" runat="server" CssClass="btn btn-danger c0l-md-4 form-control pull-left" OnClick="Button2_Click" Text="CANCEL" />
			<asp:Button ID="Button1" runat="server" Font-Bold="True" CssClass="col-md-6 btn btn-success pull-right" OnClick="Button1_Click" Text="CONTINUE TO SEND" />
                                    
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

