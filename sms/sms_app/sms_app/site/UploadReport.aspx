<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="UploadReport.aspx.cs" Inherits="UploadReport" Title="VIEW File Upload Report" %>

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
        <i class="fa fa fa-file"></i> Reports <i class='fa fa-arrow-right'></i> File Upload Report
        </div>
        <div class="card-body row clearfix">
            <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="View1" runat="server">
                 <div class="col-md-2">
                        <label>Vendor</label>
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
                        <label> Report Type </label>
                            <asp:DropDownList ID="ddlReportType" runat="server" CssClass="form-control">
                              <asp:ListItem Value="0">ALL</asp:ListItem>
                             <asp:ListItem Value="ListSMS">List SMS</asp:ListItem>
                                <asp:ListItem Value="FileSMS">File SMS</asp:ListItem>
                                <asp:ListItem Value="ContactUpload">Contact Upload</asp:ListItem>
                                <asp:ListItem Value="SMSTEMPLATE"> SMS Template</asp:ListItem>
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
                    <div class="col-md-12" >
                    <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="true"
                  UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                        HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand" style="margin-top:10px;"
                        OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="30" ShowFooter="True">
                    <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <Columns>
                        <asp:ButtonColumn  HeaderText="View Phones" Text="View" CommandName="btnEdit">
                            
                        </asp:ButtonColumn>                       
                    </Columns>
                    <HeaderStyle BackColor="#4380B8"   ForeColor="White" />
                </asp:DataGrid>
                </div>
                </asp:View>

                <asp:View ID="View2" runat="server">
                    
                    <div class="col-md-3 ">
                        <label>Search Phone</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label>Serach..</label><br/>
                        <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" OnClick="btnPhone_Click" Text="Search"/>
                    </div>
                    
               
                     <table class="col-md-6 table-bordered"  style="text-align:center; margin-top:10px;">
                        <tr>
                            <td>
                                <asp:RadioButton ID="rdbtnpdf" runat="server" Text="PDF"  /></td>
                            <td>
                                <asp:RadioButton ID="rdbtnExcel" runat="server" Text="Excel"  /></td>
                            <td>
                                <asp:Button ID="btnConvert" runat="server" Text="CONVERT" CssClass="btn btn-danger" OnClick="btnConvert_Click" /></td>
                        </tr>
                    </table>

                    <asp:DataGrid ID="DataGrid2" runat="server" AllowPaging="True" AutoGenerateColumns="false"
                  UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                        HorizontalAlign="Justify" style="margin-top:10px;"
                        OnPageIndexChanged="DataGrid2_PageIndexChanged" PageSize="30" ShowFooter="True">
                    <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <Columns>
                        <asp:BoundColumn DataField="PhoneNumber" HeaderText="Phone Number">  
                        </asp:BoundColumn>  
                        <asp:BoundColumn DataField="Status" HeaderText="Status">  
                        </asp:BoundColumn>   
                        <asp:BoundColumn DataField="Reason" HeaderText="Reason">  
                        </asp:BoundColumn>   
                        <asp:BoundColumn DataField="RecordDate" HeaderText="Record Date">  
                        </asp:BoundColumn>               
                    </Columns>
                    <HeaderStyle BackColor="#4380B8"   ForeColor="White" />
                </asp:DataGrid>
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
</div>
</asp:Content>