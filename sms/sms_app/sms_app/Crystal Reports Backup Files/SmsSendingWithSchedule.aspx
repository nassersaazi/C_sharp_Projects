<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="SmsSendingWithSchedule.aspx.cs" Inherits="SmsSending" Title="SMS SENDING PANEL" %>
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
        <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i> Schedule SMS
        
        </div>
        <div class="card-body">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="3">

    <asp:View ID="View1" runat="server">
        
            <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
				 
                <div class="modal-body">
                        <label>Select Receipient</label>
							<div class="form-group input-group">
								
								<asp:DropDownList ID="ddlReceipient" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlReceipient_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                    <asp:ListItem Value="0">Send to Number(s)</asp:ListItem>
                                    <asp:ListItem Value="1">Send to Group</asp:ListItem>
                                    <asp:ListItem Value="2">Upload contact File</asp:ListItem>
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
                            <label>Upload Contact file</label>
							<div class="form-group input-group">
								
								<asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                            </div> 
                        </asp:View>
                </asp:MultiView>


                            <label>Message Template</label>
							<div class="form-group input-group">
								
								<asp:DropDownList ID="ddlMessageTemplates" runat="server" CssClass="form-control"
                                 OnSelectedIndexChanged="ddlMessageTemplates_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                    <%--<asp:ListItem Value="">Select Message Template</asp:ListItem>--%>
                                </asp:DropDownList>
                            </div>


							<label> Message To Send</label>
							<div class="form-group input-group">
								<asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" Height="100" TextMode="MultiLine" onKeyDown="textCounter(document.txtMessage,document.txtCount,125)"
                                        onKeyUp="textCounter(document.txtMessage,document.txtCount,125)"></asp:TextBox>
                            </div>
                            <asp:Label ID="lblMessageLength" runat="server" Font-Bold="True" Text="SMS MESSAGE LENGTH : 160"></asp:Label> <br/>
                            Count: <span id="spanDisplay"> </span>
                      
				            </div>
							<div class="modal-footer">
					
					            <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success pull-right" OnClick="btnOK_Click" Text="SEND MESSAGE" />
					        </div>
				 <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
                                 TargetControlID="txtPhones" ValidChars="0123456789,+" Enabled="True">
                 </ajaxToolkit:FilteredTextBoxExtender>
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
			<div><label>List To Send To</label></div>
        <div class="form-group input-group">
			<asp:TextBox ID="txtviewlistname" runat="server" CssClass="form-control" Enabled="False"></asp:TextBox>
		</div> 
        <span style="display:none">
		    <label>Contact To send</label>
		    <div class="form-group input-group">
			    <asp:TextBox ID="txtviewprefix" runat="server" CssClass="form-control" Enabled="False"></asp:TextBox>
            </div>
        </span> 
		<label>Message To Send</label>
		<div class="form-group input-group">
			<asp:TextBox ID="txtViewMessage" runat="server" CssClass="form-control" TextMode="MultiLine"  Enabled="False" ReadOnly="True"></asp:TextBox>
        </div>
        Count: <asp:Label ID="TextBox3" runat="server"></asp:Label>
		</div>
		<div class="modal-footer">
        <asp:Button ID="Button2" runat="server" CssClass="btn btn-danger form-control pull-left" OnClick="Button2_Click" Text="CANCEL" />
			<asp:Button ID="Button1" runat="server" Font-Bold="True" CssClass="btn btn-success pull-right" OnClick="Button1_Click" Text="CONTINUE TO SEND" />
                                    
		</div>
	</div>

      </asp:View>
      

      <asp:View ID="View7" runat="server">

      <div class=" modal-content col-md-6  col-sm-6 col-xs-10" style="margin:0 auto;">
		<div class="modal-header">
			<center><h4>SMS SCHEDULE</h4></center>
		</div>
		<div class="modal-body">

        <div class="form-group input-group">

        <div class="row">
           <div class="col-sm-4">
                <asp:Label Width="200px" ID="Label3" runat="server" Text="HOUR: "></asp:Label>
                <asp:DropDownList Width="100px" ID="DropDownList2" runat="server" >
                    <asp:ListItem>00</asp:ListItem>
                </asp:DropDownList>
           </div>


           <div class="col-sm-4">
               <asp:Label Width="200px" ID="Label4" runat="server" Text="MINUTE:"></asp:Label>
                <asp:DropDownList Width="100px" ID="DropDownList3" runat="server">
                </asp:DropDownList>
           </div>


           <div class="col-sm-4">
                <asp:CheckBox ID="CheckBox1" runat="server" oncheckedchanged="CheckBox1_CheckedChanged" Text="REPEAT" />
           </div>

        </div>

            
		</div>

        <div class="col-sm-12" style="margin-top:15px">
            <p>REPEAT ON</p>
        </div>

            <div class="form-group input-group">
                <asp:CheckBoxList ID="CheckBoxList1" runat="server">
                    <asp:ListItem>MON</asp:ListItem>
                    <asp:ListItem>TUE</asp:ListItem>
                    <asp:ListItem>WED</asp:ListItem>
                    <asp:ListItem>THUR</asp:ListItem>
                    <asp:ListItem>FRI</asp:ListItem>
                    <asp:ListItem>SAT</asp:ListItem>
                    <asp:ListItem>SUN</asp:ListItem>
                </asp:CheckBoxList>
            </div>
            &nbsp;</div>
		<div class="modal-footer">
        <asp:Button ID="Button3" runat="server" CssClass="btn btn-danger form-control pull-left" OnClick="Button2_Click" Text="CANCEL" />
			<asp:Button ID="Button4" runat="server" Font-Bold="True" CssClass="btn btn-success pull-right" OnClick="Button1_Click" Text="SAVE SCHEDULE" />
                                    
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

