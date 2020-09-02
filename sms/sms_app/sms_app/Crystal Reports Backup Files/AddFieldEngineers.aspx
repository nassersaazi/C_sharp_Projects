<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="AddFieldEngineers.aspx.cs" Inherits="Areas" Title="Field Engineers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>
 <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" />
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <table align="center" style="width: 90%">
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        <table align="center" cellpadding="0" cellspacing="0" style="width: 50%">
                            <tr>
                                <td class="InterfaceHeaderLabel" style="height: 20px">
                                    FIELD Engineers
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 2px; text-align: right">
                        <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel Upload" />
                        <asp:Button ID="btnfiedEngineer" runat="server" OnClick="btnfiedEngineer_Click" Text="Upload Field Engineers List" /></td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        &nbsp;<asp:MultiView ID="MultiView3" runat="server">
                            <asp:View ID="View3" runat="server">
                                <table align="center" cellpadding="0" cellspacing="0" class="style12" style="border-right: #617da6 1px solid;
                            border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid"
                            width="60%">
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left; width: 444px;">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                                <tbody>
                                                    <tr>
                                                        <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center">
                                                            <asp:Label ID="lblConferenceName" runat="server" Text="ADD/EDIT ENGINEER"></asp:Label></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left; width: 444px;">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 444px; height: 4px; text-align: center">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 80%">
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                Name</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 19px">
                                                <asp:TextBox ID="txtName" runat="server" Width="90%"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                Telephone</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 19px">
                                                <asp:TextBox ID="txtPhone" runat="server" Width="90%"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                District</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 19px">
                                                <asp:DropDownList ID="ddlLocation" runat="server" Width="90%" OnDataBound="ddlLocation_DataBound">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="height: 5px">
                                                <asp:MultiView ID="MultiView4" runat="server">
                                                    <asp:View ID="View5" runat="server">
                                                        <table align="center" cellpadding="0" cellspacing="0" style="width: 100%">
                                                            <tr>
                                                                <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                                </td>
                                                                <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                                                </td>
                                                                <td class="InterFaceTableRightRow" style="height: 19px; background-color: white;">
                                                                    <strong><span style="font-size: 15px; font-family: Cambria">
                                                                        <asp:CheckBox ID="ChkActive" runat="server" Text="Active" /></span></strong></td>
                                                            </tr>
                                                        </table>
                                                    </asp:View>
                                                </asp:MultiView></td>
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
                                                <asp:Button ID="Button1" runat="server" Font-Bold="True" Text="Save Details" OnClick="Button1_Click" /></td>
                                        </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 444px; height: 5px; text-align: center">
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View4" runat="server">
                                <table align="center" cellpadding="0" cellspacing="0" class="style12" style="border-right: #617da6 1px solid;
                            border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid"
                            width="60%">
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left; width: 444px;">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                                <tbody>
                                                    <tr>
                                                        <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center">
                                                            <asp:Label ID="Label2" runat="server" Text="Upload File"></asp:Label></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left; width: 444px;">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 444px; height: 4px; text-align: center">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 80%">
                                                <tr>
                                                    <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                        Location</td>
                                                    <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                                    </td>
                                                    <td class="InterFaceTableRightRow" style="height: 19px">
                                                        <asp:DropDownList ID="ddlLocation2" runat="server" Width="90%" OnDataBound="ddlLocation2_DataBound">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="height: 5px">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="height: 5px">
                                                        Select &nbsp;File:<asp:FileUpload ID="FileUpload1" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="height: 5px">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="InterFaceTableLeftRowUp" style="height: 20px">
                                                    </td>
                                                    <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 20px;">
                                                    </td>
                                                    <td class="InterFaceTableRightRow" style="height: 20px">
                                                        <asp:Button ID="btnUpload" runat="server" Font-Bold="True" Text="Upload File" OnClick="btnUpload_Click" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; width: 444px; height: 5px; text-align: center">
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView></td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        <asp:MultiView ID="MultiView2" runat="server">
                            <asp:View ID="View2" runat="server">
                                <table align="center" cellpadding="0" cellspacing="0" class="style12" style="border-right: #617da6 1px solid;
                                    border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid"
                                    width="90%">
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                                <tbody>
                                                    <tr>
                                                        <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center">
                                                            <asp:Label ID="Label1" runat="server" Text="FIELD ENGINEERS LOCATION LIST"></asp:Label></td>
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
                                        <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                            <table align="center" cellpadding="0" cellspacing="0" style="border-right: #617da6 1px solid;
                                                border-top: #617da6 1px solid; border-left: #617da6 1px solid; width: 95%; border-bottom: #617da6 1px solid">
                                                <tr>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 20%; height: 18px;
                                                        text-align: center">
                                                        AREA</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 34%; height: 18px;
                                                        text-align: center">
                                                        ENTER NAME</td>
                                                    <td class="InterfaceHeaderLabel2" style="vertical-align: middle; width: 40%; height: 18px;
                                                        text-align: center">
                                                        enter CoNTACT</td>
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
                                                            OnDataBound="ddlAreas_DataBound" Width="95%">
                                                        </asp:DropDownList></td>
                                                    <td style="vertical-align: middle; width: 34%; height: 23px; text-align: center">
                                                        &nbsp;
                                                        <asp:TextBox ID="txtSearch" runat="server" Style="border-right: 1px solid; border-top: 1px solid;
                                                            border-left: 1px solid; border-bottom: 1px solid" Width="90%"></asp:TextBox></td>
                                                    <td style="vertical-align: middle; width: 40%; height: 23px; text-align: center">
                                                        <asp:TextBox ID="txtSearchContact" runat="server" Style="border-right: 1px solid;
                                                            border-top: 1px solid; border-left: 1px solid; border-bottom: 1px solid" Width="90%"></asp:TextBox></td>
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
                                                    <asp:BoundColumn DataField="RecordId" HeaderText="RecordId" Visible="False">
                                                        <HeaderStyle Width="20%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="EngineerName" HeaderText="Engineer Name" Visible="False"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="No." HeaderText="No.">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:ButtonColumn CommandName="btnEdit" HeaderText="Edit" Text="EngineerName" DataTextField="EngineerName">
                                                        <HeaderStyle Width="20%" />
                                                        <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" ForeColor="Blue" />
                                                    </asp:ButtonColumn>
                                                    <asp:BoundColumn DataField="Contact" HeaderText="Contact">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Area" HeaderText="District">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="AreaID" HeaderText="AreaID" Visible="false">
                                                        <HeaderStyle Width="10%" />
                                                    </asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Active" HeaderText="Active" Visible="true">
                                                        <HeaderStyle Width="10%" />
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
    <asp:Label ID="lblCode" runat="server" Text="0" Visible="False"></asp:Label><br />
    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
        TargetControlID="txtPhone" ValidChars="0123456789,+" Enabled="True">
    </ajaxToolkit:FilteredTextBoxExtender>
</asp:Content>

