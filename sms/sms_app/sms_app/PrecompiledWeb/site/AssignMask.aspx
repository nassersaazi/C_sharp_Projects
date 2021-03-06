﻿<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="AssignMask, App_Web_exnx0sam" title="Assign Mask" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%--<%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>--%>
 <asp:ScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" />

<div class="col-lg-12">
   
    <div class="card mb-3">
        <div class="card-header">
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> Assign Mask
        
        </div>
        <div class="card-body">
        <div class="row clearfix" style="overflow-x:auto;">
                <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">
                    <div class="modal-content col-md-4">
                        <div class="modal-header">
			                <center><h4>Assign Mask</h4></center>
		                </div>
		                <div class="modal-body">
                   

                            Vendor
                           <asp:DropDownList ID="ddlVendors" runat="server"  CssClass="form-control" >
                           </asp:DropDownList>

                            Mask
                            <asp:DropDownList ID="dllMask" runat="server"  CssClass="form-control" >
                           </asp:DropDownList>
		        
                            <br/>
                            <asp:Button ID="btnAssignMask" runat="server" CssClass="btn btn-success" Text="Save Mask" OnClick="Button1_Click" />
                        </div>
                        <div class="modal-footer">

                        

                        </div>
                </div>

                    
                    <div class="col-md-8">
                    <div class="row clearfix">
                        <div class="col-md-4">
                             Enter (Vendor or Mask)
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            Search..<br/>
                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="Button1_Ok" />
                        </div>
                    </div>
                    <asp:MultiView ID="MultiView2" runat="server">
                        <asp:View ID="View2" runat="server">
                        <asp:DataGrid ID="DataGrid1" runat="server"  AutoGenerateColumns="True" style="margin-top:10px;"
                                UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                                    HorizontalAlign="Justify" AllowPaging="True" PageSize="20"
                                OnPageIndexChanged="DataGrid1_PageIndexChanged">
                                <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                                <PagerStyle  ForeColor="#4380B8" HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                <Columns>
                                  
                                                   
                                </Columns>
                                <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" Font-Names="Courier New" />
                            </asp:DataGrid>
                            </asp:View>
                        </asp:MultiView>
                    </div>
                </asp:View>
            </asp:MultiView>
            </div>
        </div>
    </div>
</div>


</asp:Content>