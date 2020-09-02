<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="LogActivity.aspx.cs" Inherits="LogActivity" Title="USERS Activities" %>
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
        <i class="fa fa fa-file"></i> Reports <i class='fa fa-arrow-right'></i> Activity Logs
        </div>
        <div class="card-body">
            <div class="row clearfix" style="overflow-x:auto;">
                 <asp:MultiView ID="MultiView2" runat="server">
                <asp:View ID="View2" runat="server">
                <div class="col-md-2">
                    <label>Vendor</label>
                    <asp:DropDownList ID="ddlAreas" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>
               
                <div class="col-md-2">
                    <label>name(s)</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control "></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <label>Start Date</label>
                    <asp:TextBox ID="TextFromDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <label>End Date</label>
                    <asp:TextBox ID="TextToDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <label>Search..</label><br/>
                    <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search" />
                </div>


                <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="True"
                    HorizontalAlign="Justify"
                    UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                    OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True" style="white-space:nowrap; margin-top: 10px;">
                   <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                    <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                    <Columns>
                    </Columns>
                    
                </asp:DataGrid>

                </asp:View>
                </asp:MultiView>
             </div>
        </div>
      </div>
    </div> 

</asp:Content>

