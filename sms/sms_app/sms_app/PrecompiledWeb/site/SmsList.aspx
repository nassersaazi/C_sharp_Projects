<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="SmsList, App_Web_exnx0sam" title="Contact Group" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div class="col-lg-12">

            <div class="card mb-3">
                <div class="card-header">
                <i class="fa fa fa-envelope"></i> SMS PANEL <i class='fa fa-arrow-right'></i>Contact Group PANEL
                </div>
                <div class="card-body row clearfix col-md-12">
                  <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                    <asp:View ID="View1" runat="server"> 
                    <div class="modal-content col-md-3  col-sm-3 col-xs-3" style="height: 280px;">
                        <div class="modal-header">
			                <center><asp:Label ID="lblConferenceName" runat="server" Text="ADD SMS LIST"></asp:Label></center>
		                </div>
                        <div class=" modal-body">
                            
                                <label> Contact Group Name </label>
                                <asp:TextBox ID="txtListName" runat="server" CssClass="form-control"></asp:TextBox>
                             
                                
                                <label><br/></label><br/>
                                <asp:CheckBox ID="chkActive" runat="server" Text=" Is List Active" />
                                <br/><br/>

                                <label> Save </label><br/>
                                <asp:Button ID="Button1" runat="server" CssClass="btn btn-success" Text="Save List" OnClick="Button1_Click" />
                              
                        </div>
                   </div>
                   
                    <asp:MultiView ID="MultiView2" runat="server" >
                        <asp:View ID="View2" runat="server">
                        <div class="col-md-9 col-sm-9 col-xs-9">
                         
                            <div class="modal-body row clearfix" style="overflow-x:auto;">
                                <div class="col-md-4 form-group">
                                    <label>Group Name</label>
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-4 form-group">
                                    <label>Search..</label><br/>
                                    <asp:Button ID="Button2" runat="server" CssClass="btn btn-success" Text="Search" OnClick="Button2_Click" />
                                </div>
                           

                            <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False" style="white-space:nowrap;"
                                                OnItemCommand="DataGrid1_ItemCommand" OnPageIndexChanged="DataGrid1_PageIndexChanged" 
                                                 PageSize="10" UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover">

                                                <FooterStyle BackColor="InactiveCaption"   ForeColor="White" />
                                                <PagerStyle  ForeColor="#4380B8"  HorizontalAlign="Center" Mode="NumericPages"  Font-Size="16"/>

                                               
                                                <Columns>
                                                    <asp:BoundColumn DataField="ListID" HeaderText="Code" Visible="False"></asp:BoundColumn>
                                                    <asp:ButtonColumn CommandName="btnEdit" HeaderText="Edit" Text="Edit">
                                                        <%-- <ItemStyle CssClass="btn btn-info btn-sm" ForeColor="#FEFEFE"/>--%>
                                                    </asp:ButtonColumn>
                                                    <asp:ButtonColumn CommandName="btnAdd" HeaderText="Add Number" Text="Add">
                                                       <%--<ItemStyle CssClass="btn btn-danger btn-sm" ForeColor="#FEFEFE"/>--%>
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="ListName" HeaderText="Name">
                                                        
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Active" HeaderText="Active">
                                                        
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Names" HeaderText="Added By">
                                                        
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="VendorCode" HeaderText="Vendor">
                                                        
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="numbers" HeaderText="Contacts">
                                                        
                                                    </asp:BoundColumn>
                                                </Columns>
                                                <HeaderStyle BackColor="#4380B8" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
                                            </asp:DataGrid>
                                     </div>
                            </div>
                        </asp:View>
                    </asp:MultiView>
                   
                 </asp:View>
                     
                </asp:MultiView>
                </div>
                <asp:Label ID="lbllistCode" runat="server" Text="0" Visible="False"></asp:Label>
            </div>
        </div>

</asp:Content>

