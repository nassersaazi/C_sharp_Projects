<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="Sms_logs, App_Web_hiefcs1v" title="SMS LOGS" %>
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
                                    width="98%">
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left; width: 100%;">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                                <tbody>
                                                    <tr>
                                                        <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center">
                                                            <asp:Label ID="Label1" runat="server" Text="SMS LOGS"></asp:Label></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left; width: 838px;">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: center; width: 100%;">
                                            <table align="center" cellpadding="0" cellspacing="0" style="border-right: #617da6 1px solid;
                                                border-top: #617da6 1px solid; border-left: #617da6 1px solid; width: 98%; border-bottom: #617da6 1px solid">
                                                <tr>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 15%; height: 18px;
                                                        text-align: center">
                                                        AREA</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                        User</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                        list</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                        START DATE</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                        END DATE</td>
                                                </tr>
                                                <tr>
                                                    <td class="ddcolortabsline2" colspan="5" style="border-right: #617da6 1px solid;
                                                        border-top: #617da6 1px solid; vertical-align: middle; border-left: #617da6 1px solid;
                                                        border-bottom: #617da6 1px solid; height: 1px; text-align: center">
                                                        &nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="vertical-align: middle; width: 15%; height: 23px; text-align: center">
                                                        <asp:DropDownList ID="ddlAreas" runat="server" AutoPostBack="True" CssClass="InterfaceDropdownList"
                                                            OnDataBound="ddlAreas_DataBound"
                                                            Width="95%" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid" OnSelectedIndexChanged="ddlAreas_SelectedIndexChanged">
                                                        </asp:DropDownList></td>
                                                    <td style="vertical-align: middle; width: 20%; height: 23px; text-align: center"><asp:DropDownList ID="ddlUsers" runat="server" CssClass="InterfaceDropdownList"
                                                            OnDataBound="ddlUsers_DataBound"
                                                            Width="95%" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid">
                                                    </asp:DropDownList>&nbsp;</td>
                                                    <td style="vertical-align: middle; width: 20%; height: 23px; text-align: center">
                                                        <asp:DropDownList ID="ddllists" runat="server" CssClass="InterfaceDropdownList"
                                                            OnDataBound="ddllists_DataBound"
                                                            Width="95%" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid">
                                                        </asp:DropDownList></td>
                                                    <td style="vertical-align: middle; width: 20%; height: 23px; text-align: center">
                                                        <asp:TextBox ID="txtstartdate" runat="server" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid" Width="95%"></asp:TextBox></td>
                                                    <td style="vertical-align: middle; width: 20%; height: 23px; text-align: center">
                                                        <asp:TextBox ID="txtenddate" runat="server" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid" Width="95%"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="5" style="vertical-align: middle; height: 10px; text-align: center">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="5" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid;
                                                        vertical-align: middle; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid;
                                                        height: 23px; text-align: center">
                                                        <asp:Button ID="btnOK" runat="server" Font-Size="9pt" Height="23px" OnClick="btnOK_Click"
                                                            Text="Search" Width="95px" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left; width: 838px;">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 100%; height: 4px; text-align: center">
                                            <table align="center" cellpadding="0" cellspacing="0" style="border-right: #617da6 1px solid;
                                                border-top: #617da6 1px solid; border-left: #617da6 1px solid; width: 98%; border-bottom: #617da6 1px solid">
                                                <tr>
                                                    <td colspan="5" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid;
                                                        vertical-align: middle; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid;
                                                        height: 23px; text-align: center">
                                            <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                                CellPadding="4" Font-Bold="False" Font-Italic="False" Font-Names="Courier New"
                                                Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ForeColor="#333333"
                                                GridLines="Horizontal" HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                                                Style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; font: menu;
                                                border-left: #617da6 1px solid; border-bottom: #617da6 1px solid; text-align: justify"
                                                Width="100%" OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True" OnSelectedIndexChanged="DataGrid1_SelectedIndexChanged">
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
                                                    <asp:BoundColumn DataField="RecordID" HeaderText="Code" Visible="False"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="ListID" HeaderText="ListID" Visible="False"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="No." HeaderText="No.">
                                                        <HeaderStyle Width="5%" />
                                                    </asp:BoundColumn>
                                                    <asp:ButtonColumn CommandName="btnView" HeaderText="View Number" Text="Click">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:ButtonColumn HeaderText="Message" Text="Click" CommandName="btnMessage">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn HeaderText="List Name" DataField="ListName">
                                                        <HeaderStyle Width="15%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="SmsMessage" HeaderText="Message" Visible="False">
                                                        <HeaderStyle Width="30px" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Mask" HeaderText="Mask">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="ListTotal" HeaderText="Cost">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle Width="120px" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CreatedBy" HeaderText="Created By">
                                                        <HeaderStyle Width="15%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date">
                                                        <HeaderStyle Width="20%" />
                                                    </asp:BoundColumn>
                                                </Columns>
                                                <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="White" Font-Names="Courier New" />
                                            </asp:DataGrid></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            &nbsp;<asp:View ID="View1" runat="server">
                                <table align="center" style="width: 90%">
                                    <tr>
                                        <td style="width: 100%; height: 2px; text-align: center">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 50%">
                                                <tr>
                                                    <td class="InterfaceHeaderLabel">
                                                        SMS MESSAGE LOG</td>
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
                                                width="85%">
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
                                                    <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; width: 100%; height: 4px; text-align: center">
                                                        <table align="center" cellpadding="0" cellspacing="0" style="width: 60%">
                                                            <tr>
                                                                <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                                    SMS Mask</td>
                                                                <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                                                </td>
                                                                <td class="InterFaceTableRightRow" style="height: 19px">
                                                                    <asp:TextBox ID="txtMask" runat="server" ReadOnly="True"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="height: 2px">
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; height: 4px; text-align: left">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                                        <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                                            <tr>
                                                                <td class="InterFaceTableLeftRowUp" colspan="3" style="width: 100%; height: 19px;
                                                                    text-align: center">
                                                                    Message Sent</td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="height: 19px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="width: 100%; text-align: center">
                                                                    <asp:TextBox ID="txtMessage" runat="server" CssClass="InterfaceTextboxLongReadOnly"
                                                                        Height="97px" ReadOnly="True" TextMode="MultiLine" Width="60%"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="InterFaceTableRightRow" colspan="3" style="width: 100%; height: 1px; text-align: center">
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        &nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; height: 2px; text-align: center">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; height: 2px; text-align: center">
                                                        <asp:Button ID="Button1" runat="server" Font-Bold="True" Font-Size="9pt" Height="23px"
                                                            OnClick="Button1_Click1" Style="font: menu" Text="RETURN" Width="150px" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View3" runat="server">
                                <table align="center" cellpadding="0" cellspacing="0" class="style12" style="border-right: #617da6 1px solid;
                                    border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid"
                                    width="85%">
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                                <tbody>
                                                    <tr>
                                                        <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center">
                                                            <asp:Label ID="Label2" runat="server" Text="."></asp:Label></td>
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
                                            <asp:Button ID="Button2" runat="server" Font-Bold="True" Font-Size="9pt" Height="23px"
                                                OnClick="Button1_Click1" Style="font: menu" Text="RETURN" Width="150px" /></td>
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
                                            <asp:DataGrid ID="DataGrid2" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                                CellPadding="4" Font-Bold="False" Font-Italic="False" Font-Names="Courier New"
                                                Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ForeColor="#333333"
                                                GridLines="Horizontal" HorizontalAlign="Justify" OnItemCommand="DataGrid1_ItemCommand"
                                                OnPageIndexChanged="DataGrid1_PageIndexChanged" PageSize="50" ShowFooter="True"
                                                Style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; font: menu;
                                                border-left: #617da6 1px solid; border-bottom: #617da6 1px solid; text-align: justify"
                                                Width="100%">
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
                                                    <asp:BoundColumn DataField="No." HeaderText="No.">
                                                        <HeaderStyle Width="5%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="PhoneNumber" HeaderText="Phone">
                                                        <HeaderStyle Width="15%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="PhoneName" HeaderText="Name">
                                                        <HeaderStyle Width="25%" />
                                                        <ItemStyle Width="120px" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CreatedBy" HeaderText="Created By">
                                                        <HeaderStyle Width="20%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date">
                                                        <HeaderStyle Width="20%" />
                                                    </asp:BoundColumn>
                                                </Columns>
                                                <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Names="Courier New"
                                                    Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
                                            </asp:DataGrid></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 100%; height: 4px; text-align: center">
                                            <asp:Button ID="Button3" runat="server" Font-Bold="True" Font-Size="9pt" Height="23px"
                                                OnClick="Button1_Click1" Style="font: menu" Text="RETURN" Width="150px" /></td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView><br />
    &nbsp;<asp:Label ID="lblPhoneCode" runat="server" Text="0" Visible="False"></asp:Label><br />
    <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" CssClass="MyCalendar"
        Enabled="True" Format="MMMM d, yyyy" PopupPosition="TopLeft" TargetControlID="txtstartdate">
    </ajaxToolkit:CalendarExtender>
    <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" CssClass="MyCalendar"
        Enabled="True" Format="MMMM d, yyyy" PopupPosition="TopLeft" TargetControlID="txtenddate">
    </ajaxToolkit:CalendarExtender>
</asp:Content>



