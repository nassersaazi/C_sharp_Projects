<%@ page language="C#" autoeventwireup="true" inherits="_Default, App_Web_bl3ycrp3" enableeventvalidation="false" culture="auto" uiculture="auto" %>


 <%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>
 <%@ Import
  Namespace="System.Threading" %>
  
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title> PEG-SMS PORTAL </title>
    <link href="scripts/WQC_stylesheet.css" rel="stylesheet" type="text/css" />
    
    <link href="scripts/globalscape.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        <br />
        <table align="center" style="border-right: #617da6 1px solid; border-top: #617da6 1px solid;
            border-left: #617da6 1px solid; width: 80%; border-bottom: #617da6 1px solid;
            background-color: white">
            <tr>
                <td style="width: 99%; text-align: center; background-color: #5dacf1;">
               <img alt="" src="Images/peg.png" style="width: 50%; height: 50px" />
                    </td>
            </tr>
            <tr>
                <td style="border-top-width: 1px; border-left-width: 1px; border-left-color: #617da6;
                    border-bottom-width: 1px; border-bottom-color: #617da6; font: menu; width: 100%;
                    border-top-color: #617da6; height: 5px; text-align: center; border-right-width: 1px;
                    border-right-color: #617da6">
                </td>
            </tr>
            <tr>
                <td style="font: menu; width: 100%;
                    height: 5px; text-align: center; border-top-width: 1px; border-left-width: 1px; border-left-color: #617da6; border-bottom-width: 1px; border-bottom-color: #617da6; border-top-color: #617da6; border-right-width: 1px; border-right-color: #617da6;">
                    <asp:Label ID="lblmsg" runat="server" Font-Bold="False" Font-Names="Arial Narrow"
                        ForeColor="Red" Style="font: menu" Text="."></asp:Label></td>
            </tr>
            <tr>
                <td style="border-top-width: 1px; border-left-width: 1px; border-left-color: #617da6;
                    border-bottom-width: 1px; border-bottom-color: #617da6; font: menu; width: 100%;
                    border-top-color: #617da6; height: 25px; text-align: center; border-right-width: 1px;
                    border-right-color: #617da6">
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; width: 100%; height: 100px; text-align: center">
                    <asp:MultiView ID="MultiView1" runat="server">
                        <asp:View ID="View1" runat="server">
                    <table align="center" style="width: 40%">
                        <tr>
                            <td style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid;
                                width: 45%; border-bottom: #617da6 1px solid">
                                <table align="center" cellpadding="0" cellspacing="0" style="width: 100%; border-top-width: 1px; border-left-width: 1px; border-left-color: #617da6; border-bottom-width: 1px; border-bottom-color: #617da6; border-top-color: #617da6; border-right-width: 1px; border-right-color: #617da6;">
                                    <tr>
                                        <td colspan="3" style="vertical-align: top; text-align: center">
                                            <table align="center" cellpadding="0" cellspacing="0" style="width: 100%">
                                                <tr>
                                                    <td class="InterfaceHeaderLabel">
                                                        USER-LOGIN</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="height: 1px">
                            </td>
                        </tr>
                        <tr>
                            <td class="InterFaceTableRightLogin" style="width: 20%; height: 30px; font-family: Cambria;">
                                Username:</td>
                            <td class="InterFaceTableMiddleRow" style="width: 1%; height: 30px">
                                &nbsp;</td>
                            <td class="InterFaceTableRightLogin" style="width: 78%; height: 30px">
                                <asp:TextBox ID="txtUsername" runat="server" Width="90%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="InterFaceTableRightLogin" style="width: 20%; height: 29px; font-family: Cambria;">
                                Password:</td>
                            <td class="InterFaceTableMiddleRow" style="width: 1%; height: 29px">
                                &nbsp;</td>
                            <td class="InterFaceTableRightLogin" style="width: 78%; height: 29px">
                                <asp:TextBox ID="txtpassword" runat="server" TextMode="Password" Width="90%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="height: 1px">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td style="width: 29%; height: 21px">
                                &nbsp;</td>
                            <td style="width: 1%; height: 21px">
                                &nbsp;</td>
                            <td class="InterFaceTableRightLogin" style="vertical-align: middle; width: 50%; height: 21px;
                                text-align: left">
                                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;<asp:Button ID="btnlogin" runat="server"
                                    OnClick="btnlogin_Click" Text="Login" Font-Bold="True" Width="89px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="InterFaceTableRightLogin" colspan="3" style="vertical-align: middle; height: 2px;
                                text-align: left">
                            </td>
                        </tr>
                    </table>
                            </td>
                        </tr>
                    </table>
                        </asp:View>
                        <asp:View ID="View2" runat="server">
                            <table align="center" style="width: 40%">
                                <tr>
                                    <td style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid;
                                width: 45%; border-bottom: #617da6 1px solid">
                                        <table align="center" cellpadding="0" cellspacing="0" style="width: 100%; border-top-width: 1px; border-left-width: 1px; border-left-color: #617da6; border-bottom-width: 1px; border-bottom-color: #617da6; border-top-color: #617da6; border-right-width: 1px; border-right-color: #617da6;">
                                            <tr>
                                                <td colspan="3" style="vertical-align: top; text-align: center">
                                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 100%">
                                                        <tr>
                                                            <td class="InterfaceHeaderLabel">
                                                                USER-PASSWORD RESET</td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="height: 1px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="InterFaceTableRightLogin" style="width: 30%; height: 30px; font-family: Cambria;">
                                                    Password:</td>
                                                <td class="InterFaceTableMiddleRow" style="width: 1%; height: 30px">
                                                    &nbsp;</td>
                                                <td class="InterFaceTableRightLogin" style="width: 70%; height: 30px">
                                                    <asp:TextBox ID="txtResetPasswd" runat="server" TextMode="Password" Width="90%"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="InterFaceTableRightLogin" style="width: 30%; height: 29px; font-family: Cambria;">
                                                    Confirm:</td>
                                                <td class="InterFaceTableMiddleRow" style="width: 1%; height: 29px">
                                                    &nbsp;</td>
                                                <td class="InterFaceTableRightLogin" style="width: 70%; height: 29px">
                                                    <asp:TextBox ID="txtResetConfirm" runat="server" TextMode="Password" Width="90%"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="InterFaceTableRightLogin" colspan="3" style="height: 1px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="InterFaceTableRightLogin" style="width: 30%; height: 29px">
                                                </td>
                                                <td class="InterFaceTableMiddleRow" style="width: 1%; height: 29px">
                                                </td>
                                                <td class="InterFaceTableRightLogin" style="width: 70%; height: 29px">
                                                    &nbsp;<asp:Button ID="btnchange" runat="server" OnClick="btnchange_Click" Text="Change" Font-Bold="True" Width="95px" />
                                                    <asp:Button ID="btnCancel" runat="server" Font-Bold="True" Font-Size="9pt" Height="23px"
                                    OnClick="btnCancel_Click" Text="Cancel" Width="83px" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:View>
                    </asp:MultiView>
                    <asp:Label ID="lblUsercode" runat="server" Text="0" Visible="False"></asp:Label></td>
            </tr>
            <tr>
                <td style="vertical-align: top; width: 100%; height: 20px; text-align: center">
                </td>
            </tr>
            <tr>
                <td style="border-right: #617da6 1px solid; border-top: #617da6 1px solid; font: menu;
                    border-left: #617da6 1px solid; width: 100%; color: blue; border-bottom: #617da6 1px solid;
                    height: 20px; text-align: center">
                    <a href="#">© 2012,Pegasus-techologies</a></td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
