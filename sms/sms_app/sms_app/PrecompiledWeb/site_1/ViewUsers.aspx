<%@ page language="C#" masterpagefile="~/MasterSetting.master" autoeventwireup="true" inherits="ViewUsers, App_Web_mibjsheb" title="VIEW USERS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>
 <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" />
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
                                                            <asp:Label ID="Label1" runat="server" Text="SMS LISTS DETAILS"></asp:Label></td>
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
                                            <table align="center" cellpadding="0" cellspacing="0" style="border-right: #617da6 1px solid;
                                                border-top: #617da6 1px solid; border-left: #617da6 1px solid; width: 95%; border-bottom: #617da6 1px solid">
                                                <tr>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                        AREA</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                        User type</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 40%; height: 18px;
                                                        text-align: center">
                                                        enter name</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="ddcolortabsline2" colspan="4" style="border-right: #617da6 1px solid;
                                                        border-top: #617da6 1px solid; vertical-align: middle; border-left: #617da6 1px solid;
                                                        border-bottom: #617da6 1px solid; height: 1px; text-align: center">
                                                        &nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="vertical-align: middle; width: 20%; height: 23px; text-align: center">
                                                        <asp:DropDownList ID="ddlAreas" runat="server" AutoPostBack="True" CssClass="InterfaceDropdownList"
                                                            OnDataBound="ddlAreas_DataBound"
                                                            Width="95%">
                                                        </asp:DropDownList></td>
                                                    <td style="vertical-align: middle; width: 20%; height: 23px; text-align: center"><asp:DropDownList ID="ddlUserType" runat="server" AutoPostBack="True" CssClass="InterfaceDropdownList"
                                                            OnDataBound="ddlUserType_DataBound"
                                                            Width="95%">
                                                    </asp:DropDownList>&nbsp;</td>
                                                    <td style="vertical-align: middle; width: 40%; height: 23px; text-align: center">
                                                        <asp:TextBox ID="txtSearch" runat="server" Style="border-right: 1px solid; border-top: 1px solid;
                                                            border-left: 1px solid; border-bottom: 1px solid" Width="90%"></asp:TextBox></td>
                                                    <td style="vertical-align: middle; width: 20%; height: 23px; text-align: center">
                                                        <asp:Button ID="btnOK" runat="server" Font-Size="9pt" Height="23px" OnClick="btnOK_Click"
                                                            Text="Search" Width="85px" />&nbsp;</td>
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
                                                Width="100%" OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True">
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
                                                    <asp:BoundColumn DataField="UserId" HeaderText="Code" Visible="False"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Username" HeaderText="Username" Visible="False"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="No." HeaderText="No.">
                                                        <HeaderStyle Width="5%" />
                                                    </asp:BoundColumn>
                                                    <asp:ButtonColumn CommandName="btnCredit" HeaderText="Credit" Text="Username" DataTextField="Username">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:ButtonColumn DataTextField="Username" HeaderText="Edit" Text="Username" CommandName="btnEdit">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="fullName" HeaderText="Name">
                                                        <HeaderStyle Width="25%" />
                                                        <ItemStyle Width="120px" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn HeaderText="Phone" Visible="False">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="UserType" HeaderText="Type">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Area" HeaderText="Area">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Active" HeaderText="Active">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CreationDate" HeaderText="Date" Visible="False">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                </Columns>
                                                <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" Font-Names="Courier New" />
                                            </asp:DataGrid></td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View1" runat="server">
                                <table align="center" style="width: 90%">
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
                                                                        <asp:Label ID="lbltitle" runat="server" Text="."></asp:Label></td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; height: 5px; text-align: center" align="center">
                                                        <asp:Label ID="lblCredit" runat="server" Font-Bold="True" Font-Names="Arial" ForeColor="Blue"
                                                            Text="."></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; width: 100%; height: 4px; text-align: center">
                                                        <table align="center" cellpadding="0" cellspacing="0" style="width: 65%">
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="height: 2px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableLeftRowUp" style="height: 20px">
                                                                    UserName</td>
                                                                <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 20px">
                                                                </td>
                                                                <td class="InterFaceTableRightRow" style="height: 20px">
                                                                    <asp:TextBox ID="txtUserName" runat="server" MaxLength="12" Width="60%" Enabled="False"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="height: 5px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableLeftRowUp" style="height: 20px">
                                                                    Name</td>
                                                                <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 20px">
                                                                </td>
                                                                <td class="InterFaceTableRightRow" style="height: 20px">
                                                                    <asp:TextBox ID="txtName" runat="server" Width="60%" Enabled="False"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="height: 5px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableLeftRowUp" style="height: 20px">
                                                                    Credit</td>
                                                                <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 20px">
                                                                </td>
                                                                <td class="InterFaceTableRightRow" style="height: 20px">
                                                                    <asp:TextBox ID="txtCredit" runat="server" Width="60%"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="height: 15px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableLeftRowUp">
                                                                </td>
                                                                <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                                                </td>
                                                                <td class="InterFaceTableRightRow">
                                                                    <asp:Button ID="Button1" runat="server" Font-Bold="True" OnClick="Button1_Click"
                                                                        Text="Add Credit" /></td>
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
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView><br />
    &nbsp;<asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />
    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
        TargetControlID="txtCredit" ValidChars="0123456789">
    </ajaxToolkit:FilteredTextBoxExtender>
</asp:Content>



