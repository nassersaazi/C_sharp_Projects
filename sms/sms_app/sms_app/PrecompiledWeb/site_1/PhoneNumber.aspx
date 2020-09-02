<%@ page language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="PhoneNumber, App_Web_3xzxrege" title="ADD PHONE NUMBER(S)" %>
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
                                <td class="InterfaceHeaderLabel">
                                    SMS PHONE NUMBER PANEL</td>
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
                                                    <asp:Label ID="lblConferenceName" runat="server" Text="ADD PHONE NUMBERS TO SMS LIST"></asp:Label></td>
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
                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 65%">
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 19px">
                                                List Name</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 19px">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 19px">
                                                <asp:DropDownList ID="ddllists" runat="server" OnDataBound="ddllists_DataBound" Width="95%">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableRightRow" colspan="3" style="height: 2px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 20px">
                                                Enter Phone Number</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 20px;">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 20px">
                                                <asp:TextBox ID="txtPhoneNumber" runat="server" Width="60%" MaxLength="12"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableRightRow" colspan="3" style="height: 5px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp" style="height: 20px">
                                                Enter Name</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%; height: 20px">
                                            </td>
                                            <td class="InterFaceTableRightRow" style="height: 20px">
                                                <asp:TextBox ID="txtName" runat="server" Width="60%"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableRightRow" colspan="3" style="height: 15px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                Browse File</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:FileUpload ID="FileUpload1" runat="server" Width="95%" /></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableRightRow" colspan="3">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                            </td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:Button ID="Button1" runat="server" Font-Bold="True" Text="Upload Number(s)" OnClick="Button1_Click" /></td>
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
        <asp:View ID="View2" runat="server">
            <table align="center" border="0" cellpadding="0" cellspacing="0" class="InterfaceInforTable2 "
                style="width: 90%">
                <tr style="color: #000000">
                    <td class="InterfaceInforFonts21" colspan="4" style="vertical-align: top; height: 2px;
                        background-color: white; text-align: left">
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td class="InterFaceTableLeftRowUp" colspan="4" style="vertical-align: top; width: 100%;
                        text-align: center">
                        <asp:Label ID="lblQn" runat="server" Font-Bold="True" ForeColor="Maroon" Text="."></asp:Label><asp:Button
                            ID="btnYes" runat="server" Font-Bold="True" OnClick="btnYes_Click" Text="Yes"
                            Width="106px" /><asp:Button ID="btnNo" runat="server" Font-Bold="True" OnClick="btnNo_Click"
                                Text="No" Width="94px" /></td>
                </tr>
                <tr style="color: #000000">
                    <td class="InterfaceInforFonts21" colspan="4" style="vertical-align: top; width: 100%;
                        height: 2px; background-color: white; text-align: center">
                    </td>
                </tr>
                <tr style="color: #000000">
                    <td class="InterfaceInforFonts22 InterfaceTableColor" colspan="4" style="padding-bottom: 25px;
                        vertical-align: top; padding-top: 25px; height: 20px; text-align: center">
                    </td>
                </tr>
            </table>
        </asp:View>
        &nbsp; &nbsp; &nbsp;&nbsp;
    </asp:MultiView>
    <ajaxToolkit:FilteredTextBoxExtender id="FilteredTextBoxExtender1" runat="server"
        TargetControlID="txtPhoneNumber" ValidChars="0123456789">
    </ajaxToolkit:FilteredTextBoxExtender>
    <asp:Label ID="lblPath" runat="server" Text="." Visible="False"></asp:Label>
</asp:Content>

