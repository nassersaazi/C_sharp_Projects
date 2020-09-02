<%@ Page Title="SMS" Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="MultipleContactListsSend.aspx.cs" Inherits="MultipleContactListsSend" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 <asp:ScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" 
        />
<div class="col-lg-12">
   
    <div class="card mb-3">
        <div class="card-header">
        <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i> Send SMS 
    
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
                        <label> Search Using Name Of List</label>
							<div class="form-group input-group">
									
							<%--<div class="form-group">--%>
								<asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged" placeholder="Search Specific Group" AutoPostBack="true" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        <asp:MultiView ID="MultiView2" runat="server">
                            <asp:View runat="server" ID="resultView">
                                <label>Contact Lists</label>
                                <div class="row">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" Width="100%" CssClass="table table-bordered table-hover" ID="dataGridResults" OnRowCommand="dataGridResults_RowCommand" OnPageIndexChanging="DataGrid1_PageIndexChanged" AllowPaging="true" PageSize="10" OnSelectedIndexChanged="dataGridResults_SelectedIndexChanged">
                                            <AlternatingRowStyle BackColor="#BFE4FF" />
                                            <HeaderStyle BackColor="#0375b7" Font-Bold="false" ForeColor="white" Font-Italic="False"
                                                Font-Overline="False" Font-Strikeout="False" Font-Underline="False" Height="30px" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Select All">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkboxSelectAll" Text="Select All" runat="server" AutoPostBack="true" OnCheckedChanged="dataGridResults_SelectedIndexChanged" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="CheckBox" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                
                                            </Columns>
                                            <Columns>
                                                  <asp:TemplateField HeaderText="Edit">
                                                    <ItemTemplate>
                                                     <asp:Button runat="server" CommandName="btnEdit" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' Text="Edit">
                                                    </asp:Button>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </asp:View>
                </asp:MultiView>
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

			</div>

                <div class="modal-content col-md-4  col-sm-4 col-xs-4 " style="margin-left:40px;">
                <div class="modal-header">
                 <h5>Sending SMS  Guidelines   </h5>
                </div>
                <asp:MultiView ID="MultiView4" runat="server">

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

