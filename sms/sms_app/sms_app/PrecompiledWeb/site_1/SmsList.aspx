<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="SmsList, App_Web_mibjsheb" title="LISTS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <table align="center" style="width: 90%">
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        <table align="center" cellpadding="0" cellspacing="0" style="width: 50%">
                            <tr>
                                <td class="InterfaceHeaderLabel">
                                    SMS LIST PANEL</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        <table align="center" cellpadding="0" cellspacing="0" class="style12" style="border-right: #617da6 1px solid;
                            border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid"
                            width="92%">
                            <tr>
                                <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                        <tbody>
                                            <tr>
                                                <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center">
                                                    <asp:Label ID="lblConferenceName" runat="server" Text="ADD/EDIT SMS LIST"></asp:Label></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; width: 100%; height: 4px; text-align: center">
                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 60%">
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                List Name</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 19px">
                                                <asp:TextBox ID="txtListName" runat="server" Width="90%"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="height: 5px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                Active</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:CheckBox ID="chkActive" runat="server" Text="Is List Active" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="height: 5px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                            </td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:Button ID="Button1" runat="server" Font-Bold="True" Text="Save List" OnClick="Button1_Click" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        <asp:MultiView ID="MultiView2" runat="server">
                            <asp:View ID="View2" runat="server">
                                <table align="center" cellpadding="0" cellspacing="0" class="style12" style="border-right: #617da6 1px solid;
                                    border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid"
                                    width="92%">
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                                <tbody>
                                                    <tr>
                                                        <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center">
                                                            <asp:Label ID="Label1" runat="server" Text="SMS LISTS"></asp:Label></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: center">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 80%">
                                                <tr>
                                                    <td class="InterFaceTableLeftRowUp" style="height: 19px; width: 25%;">
                                                        List Name</td>
                                                    <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                                    </td>
                                                    <td class="InterFaceTableRightRow" style="height: 19px; width: 40%;">
                                                        <asp:TextBox ID="txtSearch" runat="server" Width="90%"></asp:TextBox></td>
                                                    <td class="InterFaceTableRightRow" style="width: 20%; height: 19px">
                                                        <asp:Button ID="Button2" runat="server" Font-Bold="True" Text="Search" OnClick="Button2_Click" Width="95%" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 100%; height: 4px; text-align: center">
                                            <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                                CellPadding="4" Font-Bold="False" Font-Italic="False" Font-Names="Courier New"
                                                Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ForeColor="#333333"
                                                GridLines="Horizontal" HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                                                Style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; font: menu;
                                                border-left: #617da6 1px solid; border-bottom: #617da6 1px solid; text-align: justify"
                                                Width="100%" OnPageIndexChanged="DataGrid1_PageIndexChanged">
                                                <FooterStyle BackColor="InactiveCaption" Font-Bold="False" ForeColor="White" />
                                                <EditItemStyle BackColor="#999999" />
                                                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <PagerStyle BackColor="#003366" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" HorizontalAlign="Center"
                                                    Mode="NumericPages" />
                                                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                                                <ItemStyle BackColor="White" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="#333333" />
                                                <Columns>
                                                    <asp:BoundColumn DataField="ListID" HeaderText="Code" Visible="False"></asp:BoundColumn>
                                                    <asp:ButtonColumn CommandName="btnEdit" HeaderText="Edit" Text="Click">
                                                        <HeaderStyle Width="5%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:ButtonColumn CommandName="btnAdd" HeaderText="Add Number" Text="Click">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="ListName" HeaderText="Name">
                                                        <HeaderStyle Width="30%" />
                                                        <ItemStyle Width="120px" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Active" HeaderText="Active">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CreatedBy" HeaderText="Created By">
                                                        <HeaderStyle Width="20%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date">
                                                        <HeaderStyle Width="20%" />
                                                    </asp:BoundColumn>
                                                </Columns>
                                                <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" Font-Names="Courier New" />
                                            </asp:DataGrid></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView></td>
                </tr>
            </table>
        </asp:View>
        &nbsp; &nbsp;&nbsp;
    </asp:MultiView>
    <asp:Label ID="lbllistCode" runat="server" Text="0" Visible="False"></asp:Label>
</asp:Content>

