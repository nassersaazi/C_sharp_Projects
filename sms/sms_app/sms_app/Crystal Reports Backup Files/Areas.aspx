<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="Areas.aspx.cs" Inherits="Areas" Title="AREAS" %>
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
        <i class="fa fa fa-cog"></i> System Tools <i class='fa fa-arrow-right'></i> Areas
        </div>
        <div class="card-body">
            <div class="row clearfix">

             <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="View1" runat="server">

                    <div class="modal-content col-md-8  col-sm-8 col-xs-10"  style="margin:0 auto;">
                        <div class="modal-header">
			                <center><h4>Add Area</h4></center>
		                </div>
		                <div class="modal-body">
                   
                            Area
                            <asp:TextBox ID="txtArea" runat="server" CssClass="form-control"></asp:TextBox>
		        

                        Marsk
                        <asp:TextBox ID="txtMask" runat="server" CssClass="form-control"></asp:TextBox>
                        <br/>
                        <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" Text="Save Area" OnClick="Button1_Click" />

                        </div>
                        <div class="modal-footer">
                            <asp:MultiView ID="MultiView2" runat="server">
                                <asp:View ID="View2" runat="server">
                                   

                                     <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                               UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                                                 HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                                                OnPageIndexChanged="DataGrid1_PageIndexChanged">
                                                <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                                                <PagerStyle  ForeColor="#4380B8" HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>
                                                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                 <Columns>
                                                    <asp:BoundColumn DataField="AreaID" HeaderText="AreaID" Visible="False">
                                                        <HeaderStyle Width="20%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Area" HeaderText="Area" Visible="False"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="No." HeaderText="No.">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:ButtonColumn CommandName="btnEdit" HeaderText="Edit" Text="Area" DataTextField="Area">
                                                        <HeaderStyle Width="20%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="Mask" HeaderText="Mask">
                                                        <HeaderStyle Width="20%" />
                                                        <ItemStyle Width="120px" />
                                                    </asp:BoundColumn>
                                                </Columns>
                                                <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" Font-Names="Courier New" />
                                            </asp:DataGrid>

                                </asp:View>
                        </asp:MultiView>
                        </div>
                </div>
                </asp:View>
       
    </asp:MultiView>
            </div>
        </div>
    </div>
</div>
    <asp:Label ID="lblCode" runat="server" Text="0" Visible="False"></asp:Label><br />
</asp:Content>

