<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="ViewCreditLog, App_Web_exnx0sam" title="VIEW CREDIT LOG" %>

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
        <i class="fa fa fa-file"></i> Reports <i class='fa fa-arrow-right'></i> Credit Log
        </div>
        <div class="card-body">
            <div class="row clearfix">
                <asp:MultiView ID="MultiView2" runat="server">
                    <asp:View ID="View2" runat="server">
                        <div class="col-md-2">
                            <label>Vendor</label>
                            <asp:DropDownList ID="ddlAreas" runat="server"  CssClass="form-control"
                            OnDataBound="ddlAreas_DataBound" AutoPostBack="true" OnSelectedIndexChanged="ddlAreas_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label>User</label>
                            <asp:DropDownList ID="ddlUsers" runat="server" CssClass="form-control" OnDataBound="ddlUsers_DataBound" >
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
                            <label>Search..</label><br/>
                            <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search" />
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


                        <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="true"
                         HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                        UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                       OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True" OnSelectedIndexChanged="DataGrid1_SelectedIndexChanged">
                        <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                        <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                        <Columns>
                            
                        </Columns>
                        <HeaderStyle BackColor="#4380B8" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
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

                     <asp:DataGrid ID="DataGrid2" runat="server" AllowPaging="True" AutoGenerateColumns="true"
                        HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                            UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                        OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True">
                        <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                        <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                        <Columns>
                            
                            
                        </Columns>
                       <HeaderStyle BackColor="#4380B8" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
                    </asp:DataGrid>
                    <asp:Button ID="Button3" runat="server" CssClass="btn btn-danger" OnClick="Button1_Click1" Text="RETURN" />
                    </asp:View>

                </asp:MultiView>
            </div>
        </div>
    </div>
</div>  


<asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />
    
</asp:Content>



