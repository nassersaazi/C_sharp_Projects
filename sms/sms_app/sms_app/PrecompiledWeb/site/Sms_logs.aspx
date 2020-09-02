<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="Sms_logs, App_Web_dqet1j14" title="SMS LOGS" %>
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
        <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i> SMS Logs
        </div>
        <div class="card-body">
            <div class="row clearfix">
            <asp:MultiView ID="MultiView2" runat="server">
                <asp:View ID="View2" runat="server">

                    <div class="col-md-2">
                        <label> Vendor </label>
                        <asp:DropDownList ID="ddlAreas" runat="server" AutoPostBack="True" CssClass="form-control"
                            OnDataBound="ddlAreas_DataBound" OnSelectedIndexChanged="ddlAreas_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <label> User </label>
                            <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-control" OnDataBound="ddlUsers_DataBound">
                            </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <label> Contact Group </label>
                         <asp:DropDownList ID="ddllists" runat="server" CssClass="form-control" OnDataBound="ddllists_DataBound">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <label> Start Date </label>
                        <asp:TextBox ID="txtstartdate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label> End Date </label>
                        <asp:TextBox ID="txtenddate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label> Serach.. </label><br/>
                        <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search" />
                    </div>


                     <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                        UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover"                        
                        HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand" style="white-space:nowrap;"
                            OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="30" ShowFooter="True" OnSelectedIndexChanged="DataGrid1_SelectedIndexChanged">
                        <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                        <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                            <Columns>
                            <asp:BoundColumn DataField="RecordID" HeaderText="Code" Visible="False"></asp:BoundColumn>
                            <asp:BoundColumn DataField="ListID" HeaderText="ListID" Visible="False"></asp:BoundColumn>
                            <asp:BoundColumn DataField="No." HeaderText="No.">
                                
                            </asp:BoundColumn>
                            <asp:ButtonColumn CommandName="btnView" HeaderText="View Numbers" Text="Click">
                                
                                <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                    Font-Underline="False" ForeColor="Blue" />
                            </asp:ButtonColumn>
                            <asp:ButtonColumn HeaderText="Message" Text="Click" CommandName="btnMessage">
                                
                                <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                    Font-Underline="False" ForeColor="Blue" />
                            </asp:ButtonColumn>
                            <asp:BoundColumn HeaderText="List Name" DataField="ListName">
                                
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="SmsMessage" HeaderText="Message" Visible="False">
                              
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Mask" HeaderText="Mask">
                              
                            <%--</asp:BoundColumn>
                            <asp:BoundColumn DataField="ListTotal" HeaderText="Cost">--%>
                                
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="CreatedBy" HeaderText="Created By">
                               
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date">
                                
                            </asp:BoundColumn>
                        </Columns>
                        <HeaderStyle BackColor="#4380B8" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
                    </asp:DataGrid>


                 </asp:View>

                 <asp:View ID="View1" runat="server">
                    <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
                        <div class="modal-header">
			                <center>SMS MESSAGE LOG</center>
		                </div>
		                <div class="modal-body">
                            <asp:Label ID="lbltitle" runat="server" Text="."></asp:Label><br/>
                     
                             SMS Mask
                             <asp:TextBox ID="txtMask" runat="server" CssClass="form-control" ReadOnly="True"></asp:TextBox>

                             Message Sent<br/>
                             <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="modal-footer">
          
                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-danger" OnClick="Button1_Click1"  Text="RETURN" />
                         </div>
                     </div> 
                 </asp:View>

                 <asp:View ID="View3" runat="server">
                    <center><asp:Label ID="Label2" runat="server" Text="."></asp:Label></center>
                    <asp:Button ID="Button2" runat="server" CssClass="btn btn-danger pull-right" OnClick="Button1_Click1" Text="RETURN"/>

                    <asp:DataGrid ID="DataGrid2" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                    OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True"
                    UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover">
                    <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8" HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <Columns>
                        <asp:BoundColumn DataField="No." HeaderText="No.">
                            <HeaderStyle Width="5%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="PhoneNumber" HeaderText="Phone">
                            <HeaderStyle Width="15%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="PhoneName" HeaderText="Name">
                            <HeaderStyle Width="25%" />
                            <ItemStyle Width="120px" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="CreatedBy" HeaderText="Created By">
                            <HeaderStyle Width="20%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date">
                            <HeaderStyle Width="20%" />
                        </asp:BoundColumn>
                    </Columns>
                    <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Names="Courier New"
                        Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
                </asp:DataGrid>
                <asp:Button ID="Button3" runat="server" CssClass="btn btn-danger" OnClick="Button1_Click1" Text="RETURN"/>
                  </asp:View>
                </asp:MultiView>
            </div>
        </div>
    </div>
</div>



   <asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />

</asp:Content>



