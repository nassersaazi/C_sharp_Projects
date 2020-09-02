<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="DistrictTotalSmsSent.aspx.cs" Inherits="ViewListSmsSent" Title="VIEW SMS LOG" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
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
        <i class="fa fa fa-file"></i> Reports <i class='fa fa-arrow-right'></i> Area Total
        </div>
        <div class="card-body">
            <div class="row clearfix">
            <asp:MultiView ID="MultiView2" runat="server">
                <asp:View ID="View2" runat="server">
                    <div class="col-md-2">
                        <label>Area</label>
                        <asp:DropDownList ID="ddlAreas" runat="server" AutoPostBack="True" CssClass="form-control" OnDataBound="ddlAreas_DataBound"
                        OnSelectedIndexChanged="ddlAreas_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <label>Sent</label>
                        <asp:DropDownList ID="ddlSent" runat="server" CssClass="form-control"  OnDataBound="ddlSent_DataBound">
                            <asp:ListItem Value="3">All</asp:ListItem>
                            <asp:ListItem Value="1">Sent</asp:ListItem>
                            <asp:ListItem Value="0">Not Sent</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <label>Start Date</label>
                        <asp:TextBox ID="txtstartdate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label>End Date</label>
                        <asp:TextBox ID="txtenddate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label>Serach..</label><br/>
                        <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search"/>
                    </div>

                     <table class="table-bordered" align="center" width="50%" style="text-align:center; margin-top:10px;">
                        <tr>
                            <td>
                                <asp:RadioButton ID="rdbtnpdf" runat="server" Text="PDF" AutoPostBack="True" OnCheckedChanged="rdbtnpdf_CheckedChanged" /></td>
                            <td>
                                <asp:RadioButton ID="rdbtnExcel" runat="server" Text="Excel" AutoPostBack="True" OnCheckedChanged="rdbtnExcel_CheckedChanged" /></td>
                            <td>
                                <asp:Button ID="btnConvert" runat="server" Text="CONVERT" CssClass="btn btn-danger" OnClick="btnConvert_Click" /></td>
                        </tr>
                    </table>

                    <table class="table-bordered" style="width: 100%; text-align:center; margin-top: 10px;" >
                    <tr>
                        <td><asp:Label ID="lblTotalNo" runat="server" ForeColor="Highlight"></asp:Label></td>
                        <td></td>
                        <td><asp:Label ID="Label3" runat="server" ForeColor="Highlight"></asp:Label></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="Label6" runat="server" ForeColor="Highlight"></asp:Label></td>
                        <td><asp:Label ID="Label4" runat="server" ForeColor="Highlight"></asp:Label></td>
                        <td><asp:Label ID="Label5" runat="server" ForeColor="Highlight"></asp:Label></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="Label7" runat="server" ForeColor="Highlight"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>


                 <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                  UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                        HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                        OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True" OnSelectedIndexChanged="DataGrid1_SelectedIndexChanged">
                    <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8" HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <Columns>
                        <asp:BoundColumn DataField="Str1" HeaderText="Code" Visible="False"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Str1" HeaderText="No.">
                            <HeaderStyle Width="5%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Str2" HeaderText="AreaID" Visible="True">
                            <HeaderStyle Width="10%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="PayDate" HeaderText="District">
                            <HeaderStyle Width="20%" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Amount2" HeaderText="No of SMS(<160chrs)">
                            <HeaderStyle Width="20%" />
                            <ItemStyle Width="120px" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Amount3" HeaderText="No of SMS(>160chrs)">
                            <HeaderStyle Width="20%" />
                            <ItemStyle Width="120px" />
                        </asp:BoundColumn>
                            <asp:BoundColumn DataField="Amount4" HeaderText="Total SMS(from >160chrs)">
                            <HeaderStyle Width="20%" />
                            <ItemStyle Width="120px" />
                        </asp:BoundColumn>
                            <asp:BoundColumn DataField="totSMSCount" HeaderText="Total SMS Count">
                            <HeaderStyle Width="20%" />
                            <ItemStyle Width="120px" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="totSMSCost" HeaderText="Total SMS Cost">
                            <HeaderStyle Width="20%" />
                            <ItemStyle Width="120px" />
                        </asp:BoundColumn>
                                                   
                    </Columns>
                    <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                        Font-Strikeout="False" Font-Underline="False" ForeColor="White" Font-Names="Courier New" />
                </asp:DataGrid>
                <asp:Label ID="lblTotal" runat="server" Text="." Font-Bold="True" Font-Names="Cambria" Font-Size="Medium" ForeColor="#4380B8"></asp:Label>
               <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" />
                </asp:View>

                                     <asp:View ID="View1" runat="server">
                         <div class="modal-content col-md-6  col-sm-6 col-xs-10"  style="margin:0 auto;">
                            <div class="modal-header">
			                    <center>SMS MESSAGE LOG</center>
		                    </div>
		                    <div class="modal-body">
                                 <asp:Label ID="lbltitle" runat="server" Text="."></asp:Label><br/>

                             SMS Marsk
                             <asp:TextBox ID="txtMask" runat="server" CssClass="form-control" ReadOnly="True"></asp:TextBox>
		        

                            Message sent
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>

                            </div>
                            <div class="modal-footer">
          
                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-danger" OnClick="Button1_Click1" Text="RETURN" />
                         </div>
                    </div>
                    </asp:View>

                <asp:View ID="View3" runat="server">
                    <asp:Label ID="Label2" runat="server" Text="."></asp:Label>
                    <asp:Button ID="Button2" runat="server" CssClass="btn btn-danger pull-right" OnClick="Button1_Click1" Text="RETURN"/>

                     <asp:DataGrid ID="DataGrid2" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                        HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                            UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                        OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True">
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
                            <asp:BoundColumn DataField="Cost" HeaderText="Cost">
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
                    <asp:Button ID="Button3" runat="server" CssClass="btn btn-danger" OnClick="Button1_Click1" Text="RETURN" />
                    </asp:View>

            </asp:MultiView>
            </div>
        </div>
    </div>
</div>
</asp:Content>



