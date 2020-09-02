<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="OtherSmsReport.aspx.cs" Inherits="OtherSmsReport" Title="Other SMS Reports" %>
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
        <i class="fa fa fa-file"></i> SMS Reports <i class='fa fa-arrow-right'></i> Automated SMS Report
        </div>
        <div class="card-body">
            <div class="row clearfix">
            <asp:MultiView ID="MultiView2" runat="server">
                <asp:View ID="View2" runat="server">

                    <div class="col-md-2">
                        <label> Vendor </label>
                        <asp:DropDownList ID="ddlAreas" runat="server" OnDataBound="ddlAreas_DataBound"  CssClass="form-control">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2">
                        <label> Phone </label>
                            <asp:TextBox ID="ddlPhone" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                     <div class="col-md-2">
                        <label> Status </label>
                        <asp:DropDownList ID="ddlStatus" runat="server"  CssClass="form-control">
                                <asp:ListItem Value="">All</asp:ListItem>
                                <asp:ListItem Value="SUCCESS">SUCCESS</asp:ListItem>
                                <asp:ListItem Value="PENDING">PENDING</asp:ListItem>
                                <asp:ListItem Value="FAILED">FAILED</asp:ListItem>
                                <asp:ListItem Value="REJECTED">REJECTED</asp:ListItem>
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
                    <br/>
                        <table class="table-bordered" width="50%" align="center" style="text-align:center; margin-top:10px;">
                        <tr>
                            <td>
                                <asp:RadioButton ID="rdbtnpdf" runat="server" Text="PDF" AutoPostBack="True" OnCheckedChanged="rdbtnpdf_CheckedChanged" /></td>
                            <td>
                                <asp:RadioButton ID="rdbtnExcel" runat="server" Text="Excel" AutoPostBack="True" OnCheckedChanged="rdbtnExcel_CheckedChanged" /></td>
                            <td>
                                <asp:Button ID="btnConvert" runat="server" Text="CONVERT" CssClass="btn btn-danger" OnClick="btnConvert_Click" /></td>
                        </tr>
                    </table>

                     <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="True"
                        UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover"                        
                        HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand" style="white-space:nowrap; margin-top:20px; "
                            OnPageIndexChanged="DataGrid1_SelectedIndexChanged" PageSize="30" ShowFooter="True" >
                        <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                         <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                            <Columns>
                            
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
          
                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-danger pull-right" OnClick="Button1_Click1"  Text="RETURN" />
                         </div>
                     </div> 
                 </asp:View>
                </asp:MultiView>
            </div>
        </div>
    </div>
</div>



   <asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />

</asp:Content>



