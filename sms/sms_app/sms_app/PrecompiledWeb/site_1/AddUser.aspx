<%@ page language="C#" masterpagefile="~/MasterSetting.master" autoeventwireup="true" inherits="AddUser, App_Web_mibjsheb" title="SYSTEM USER" %>
 <%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>
 <%@ Import
  Namespace="System.Threading" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


    <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
        EnableScriptLocalization="true" ID="ScriptManager1" />
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
                <table style="width: 90%" align="center">
                    <tr>
                        <td style="width: 100%; height: 2px; text-align: center">
                            <table align="center" cellpadding="0" cellspacing="0" style="width: 50%">
                                <tr>
                                    <td class="InterfaceHeaderLabel">
                                        SYSTEM USER</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100%; height: 2px; text-align: center">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100%; text-align: center; height: 2px;"><table cellpadding="0" cellspacing="0" class="style12" align="center" width="92%" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid">
                            <tr>
                                <td colspan="3" style="vertical-align: top; height: 5px; text-align: left">
                                    <table style="width: 98%" align="center" cellpadding="0" cellspacing="0" >
                                        <tbody>
                                            <tr>
                                                <td class="InterfaceHeaderLabel2" style="height: 18px; text-align: center;">
                                                    ENTER USER DETAILS</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; height: 4px; text-align: left">
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top; width: 50%; height: 5px; text-align: left">
                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                First
                                                Name</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:TextBox ID="txtfname" runat="server" CssClass="InterfaceTextboxLongReadOnly"
                                                    Width="90%"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                Last Name</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:TextBox ID="txtlname" runat="server" CssClass="InterfaceTextboxLongReadOnly"
                                                    Width="90%"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                TelePhone</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:TextBox ID="txtphone" runat="server" CssClass="InterfaceTextboxLongReadOnly"
                                                    Width="90%"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                Email</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:TextBox ID="txtemail" runat="server" CssClass="InterfaceTextboxLongReadOnly"
                                                    Width="90%"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="vertical-align: top; width: 2%; height: 10px; text-align: center">
                                </td>
                                <td style="vertical-align: top; width: 48%; height: 5px; text-align: left">
                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                Area</td>
                                            <td class="InterFaceTableMiddleRowUp">
                                            </td>
                                            <td class="InterFaceTableRightRow"><asp:DropDownList ID="ddlAreas" runat="server" OnDataBound="ddlAreas_DataBound"
                                                    Width="90%">
                                            </asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                User Type</td>
                                            <td class="InterFaceTableMiddleRowUp">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:DropDownList ID="ddlUserType" runat="server" OnDataBound="ddlUserType_DataBound"
                                                    Width="90%">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 20px">
                                                Active</td>
                                            <td class="InterFaceTableMiddleRowUp" style="height: 20px">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 20px">
                                                <asp:CheckBox ID="chkActive" runat="server" Style="font: menu" Text="Tick To Activate" /></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableRightRow" colspan="3" style="height: 20px">
                                    <asp:MultiView ID="MultiView2" runat="server">
                                        <asp:View ID="View3" runat="server"><table align="center" cellpadding="0" cellspacing="0" style="width: 98%">
                                            <tr>
                                                <td class="InterFaceTableLeftRowUp">
                                                    User Name</td>
                                                <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                                </td>
                                                <td class="InterFaceTableRightRow">
                                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="InterfaceTextboxLongReadOnly"
                                                        Width="90%"></asp:TextBox></td>
                                            </tr>
                                        </table>
                                        </asp:View>
                                        <asp:View ID="View2" runat="server">
                                            <table cellpadding="0" cellspacing="0" class="style12" align="center" width="92%" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid">
                                                <tr>
                                                    <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                                                        <asp:CheckBox ID="CheckBox1" runat="server" Font-Bold="True" Text="Tick To Reset Password" /></td>
                                                </tr>
                                            </table>
                                        </asp:View>
                                    </asp:MultiView></td>
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
                                    </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; height: 2px; text-align: center">
                                                                <asp:Button ID="btnOK" runat="server" Font-Size="9pt" Height="23px"
                                                                    Text="SUBMIT DETAILS" Width="150px" Font-Bold="True" OnClick="btnOK_Click" style="font: menu" /></td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                                </td>
                            </tr>
                        </table>
                            <table cellpadding="0" cellspacing="0" class="style12" align="center" width="92%" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid">
                                <tr>
                                    <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
        </asp:View>
        &nbsp;&nbsp;&nbsp;&nbsp;
    </asp:MultiView>
                <asp:Label ID="lblCode" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="lblUsername" runat="server" Text="0" Visible="False"></asp:Label><br />
    &nbsp;

</asp:Content>





