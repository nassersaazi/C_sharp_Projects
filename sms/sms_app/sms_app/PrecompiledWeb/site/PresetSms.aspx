<%@ page title="" language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="PresetSms, App_Web_dqet1j14" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

        <div class="col-lg-12">

            <div class="card mb-3">
                <div class="card-header">
                <i class="fa fa fa-envelope"></i> Inbound SMS <i class='fa fa-arrow-right'></i>Preset SMS
                </div>
                <div class="card-body row clearfix col-md-12">
                    <div class="modal-content col-md-4  col-sm-4 col-xs-4">
                        <div class="modal-header">
                            Preset SMS Settings
                        </div>
                            <div class="modal-body">
                                <label>Preset Precedence</label>
							   
                                <label>Preset Title</label>
                                <asp:TextBox ID="txtPreset" runat="server" CssClass="form-control"></asp:TextBox>

                                <label> Prset Variables</label>
								<asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" Height="100" TextMode="MultiLine"></asp:TextBox>
                                <b>NOTE:</b> All Variables defined should start with <b><%=variableChar %> </b>
                                <br/><asp:CheckBox ID="chkIsActive" runat="server" Text=" Is Active" />
                            </div>
                             <asp:TextBox ID="txtId" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            
                        <div class="modal-footer">
                            <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success pull-right" OnClick="btnOK_Click" Text="SAVE PRESET" />
                        </div>
                    </div>

                    <div class="modal-content col-md-8  col-sm-8 col-xs-8">
                         <asp:MultiView ID="MultiView2" runat="server">
                                <asp:View ID="View2" runat="server">

                                     <asp:DataGrid ID="DataGrid1" runat="server"  AutoGenerateColumns="True"
                                               UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover" 
                                                 HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                                                OnPageIndexChanged="DataGrid1_PageIndexChanged">
                                                <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                                                <PagerStyle  ForeColor="#4380B8" HorizontalAlign="Center"  Font-Size="16"/>
                                                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <Columns>
                                                    <asp:ButtonColumn CommandName="btnEdit" HeaderText="Edit" Text="Edit">
                                                       
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                   
                                                </Columns>
                                                <HeaderStyle BackColor="#4380B8"  ForeColor="White" />
                                            </asp:DataGrid>

                                </asp:View>
                        </asp:MultiView>

                    </div>

                </div>
            </div>
        </div>

</asp:Content>

