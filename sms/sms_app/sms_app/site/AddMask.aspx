<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="AddMask.aspx.cs" Inherits="AddMask" Title="MASKS" %>
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
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> Add Mask
        </div>
        <div class="card-body">
            <div class="row clearfix">

             <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">

                    <div class="modal-content col-md-3  col-sm-3 col-xs-3">
                        <div class="modal-header">
			                <center><h4>Add SMS Mask</h4></center>
		                </div>
		                <div class="modal-body">
                            <label>Mask Type</label>
                            <asp:DropDownList ID="ddlMaskType" runat="server" CssClass="form-control">
                            </asp:DropDownList>

                            Mask Code
                            <asp:TextBox ID="txtMask" runat="server" CssClass="form-control"></asp:TextBox>
		        

                            Mask Name
                            <asp:TextBox ID="txtMaskName" runat="server" CssClass="form-control"></asp:TextBox>
                            <br/>
                            <asp:CheckBox ID="chkActive" runat="server" Text=" Is Active" /><br/>

                            <asp:TextBox ID="txtId" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            <br/>
                        <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" Text="Save Mask" OnClick="Button1_Click" />
                        </div>
                        
                        <div class="modal-footer">

                        

                        </div>
                </div>

                <div class="col-md-9  col-sm-9 col-xs-9" style="overflow-x: auto;">
                    
                            <asp:MultiView ID="MultiView2" runat="server">
                                <asp:View ID="View2" runat="server">

                                     <asp:DataGrid ID="DataGrid1" runat="server"  AutoGenerateColumns="True"
                                               UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                                                 HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                                                OnPageIndexChanged="DataGrid1_PageIndexChanged">
                                                <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                                                <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                                                <Columns>
                                                    <asp:ButtonColumn CommandName="btnEdit" HeaderText="Edit" Text="Edit">
                                                       
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                   
                                                </Columns>
                                                <HeaderStyle BackColor="#4380B8" ForeColor="White" />
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
    <asp:Label ID="lbllistCode" runat="server" Text="0" Visible="False"></asp:Label><br />

</asp:Content>

