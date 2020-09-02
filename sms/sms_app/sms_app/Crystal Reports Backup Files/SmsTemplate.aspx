<%@ Page Language="C#" MasterPageFile="~/MasterMain.master" AutoEventWireup="true" CodeFile="SmsTemplate.aspx.cs" Inherits="SmsTemplate" Title="SMS TEMPLATE PANEL" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <table align="center" style="width: 90%">
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        <table align="center" cellpadding="0" cellspacing="0" style="width: 50%">
                            <tr>
                                <td class="InterfaceHeaderLabel">
                                    SMS TEMPLATE PANEL</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 2px; text-align: center">
                        <asp:Label ID="lblCredit" runat="server" Font-Bold="True" Font-Names="Arial" ForeColor="Blue"
                            Text="."></asp:Label></td>
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
                                                    ENTER TEMPLATE DETAILS</td>
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
                                <td style="vertical-align: top; width: 50%; height: 10px; text-align: left">
                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 98%; border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid;">
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                Upload File:</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                    <asp:FileUpload ID="FileUpload1" runat="server" Width="98%" /></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                SMS:</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <asp:TextBox ID="txtMessage" runat="server" CssClass="InterfaceTextboxLongReadOnly"
                                                    Width="98%" Height="62px" TextMode="MultiLine" onKeyDown="textCounter(document.txtMessage,document.txtCount,125)"
                                        onKeyUp="textCounter(document.txtMessage,document.txtCount,125)" style="font-family: Arial"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableRightRow" colspan="3" style="height: 1px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="InterFaceTableLeftRowUp">
                                                Maximum (160)</td>
                                            <td class="InterFaceTableMiddleRowUp" style="width: 2%">
                                            </td>
                                            <td class="InterFaceTableRightRow">
                                                <table align="center" cellpadding="0" cellspacing="0" style="width: 60%" id="">
                                                    <tr>
                                                       Count: <span id="spanDisplay"> </span>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>                             
                                <td style="vertical-align: top; width: 48%; height: 10px; text-align: left">
                                    <table align="center" cellpadding="0" cellspacing="0" style="width: 98%; border-right: #617da6 1px solid; border-top: #617da6 1px solid; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid;">
                                        <tr>
                                        
                                                <td colspan="3" class="InterFaceTableLeftRowUp" style="height: 20px" HorizontalAlign="Center">
                                                    <asp:Label ID="Label1" runat="server" Text="Charge List:"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            
                                            <td class="InterFaceTableRightRow" style="height: 20px">
                                                <asp:DataGrid ID="DataGrid1" runat="server" AutoGenerateColumns="False" CellPadding="1"
                                                    Font-Bold="False" Font-Italic="False" Font-Names="Courier New" Font-Overline="False"
                                                    Font-Strikeout="False" Font-Underline="False" ForeColor="#333333" GridLines="Horizontal"
                                                    HorizontalAlign="Justify" PageSize="5" Style="border-right: #617da6 1px solid;
                                                    border-top: #617da6 1px solid; font: menu; border-left: #617da6 1px solid; border-bottom: #617da6 1px solid;
                                                    text-align: justify" Width="100%">
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
                                                        <asp:BoundColumn DataField="Network" HeaderText="Network">
                                                            <HeaderStyle Width="10%" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Rate(UShs.)" HeaderText="Rate(UShs.)">
                                                            <HeaderStyle Width="20%" />
                                                            <ItemStyle Width="120px" />
                                                        </asp:BoundColumn>
                                                    </Columns>
                                                    <HeaderStyle BackColor="#006699" Font-Bold="True" Font-Italic="False" Font-Names="Courier New"
                                                        Font-Overline="False" Font-Strikeout="False" Font-Underline="False" ForeColor="White" />
                                                </asp:DataGrid></td>
                                        </tr>
                                    </table>
                                    </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; height: 1px; text-align: center">
                                <hr/>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="vertical-align: top; height: 2px; text-align: center">
                                    <asp:Button ID="btnOK" runat="server" Font-Bold="True" Font-Size="9pt" Height="23px"
                                        OnClick="btnOK_Click" Style="font: menu" Text="SEND MESSAGE" Width="150px" /></td>
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
        &nbsp;&nbsp;
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
                                        SMS CREDIT MESSAGE</td>
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
                    <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center" class="InterFaceTableLeftRowUp">
                        &nbsp;<asp:Label ID="lblerror" runat="server" Font-Bold="True" ForeColor="#C00000"
                            Text="."></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="3" style="vertical-align: top; height: 2px; text-align: center">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="vertical-align: top; width: 100%; height: 5px; text-align: center">
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
<script type="text/javascript">
  function count(clientId) {
    var txtMessage = document.getElementById(clientId);
    var spanDisplay = document.getElementById('spanDisplay');
    spanDisplay.innerHTML = txtMessage.value.length;
  }
</script>

</asp:Content>

