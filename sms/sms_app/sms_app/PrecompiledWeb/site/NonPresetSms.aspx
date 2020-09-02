<%@ page title="" language="C#" masterpagefile="~/MasterMain.master" autoeventwireup="true" inherits="NonPresetSms, App_Web_exnx0sam" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="col-lg-12">

            <div class="card mb-3">
                <div class="card-header">
                <i class="fa fa fa-file"></i> Reports<i class='fa fa-arrow-right'></i>Preset SMS Report
                </div>
                <div class="card-body row clearfix col-md-12">
                 <div class="col-md-2">
                            <label> Vendor </label>
                             <asp:DropDownList ID="ddlAreas" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                       
                        
                        <div class="col-md-2">
                            <label> Status </label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">ALL</asp:ListItem>
                                 <asp:ListItem Value="PENDING">PENDIG</asp:ListItem>
                                <asp:ListItem Value="SUCCESS">SUCCESS</asp:ListItem>
                                <asp:ListItem Value="FAILED">FAILED</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label> Start Date </label>
                            <asp:TextBox ID="txtstartdate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label> End date </label>
                            <asp:TextBox ID="txtenddate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                        </div>
                        <div class="col-md-2">
                            <label> Search.. </label><br/>
                            <asp:Button ID="btnOK" runat="server" CssClass="btn btn-success" OnClick="btnOK_Click" Text="Search" />
                        </div>
                        <br/>
                       

                <div class="col-md-12  col-sm-12 col-xs-12" style="margin-top:10px;">
                     <table class="table-bordered" width="50%" align="center" style="text-align:center; margin-top:10px;">
                        <tr>
                            <td>
                                <asp:RadioButton ID="rdbtnpdf" runat="server" Text="PDF" AutoPostBack="True" OnCheckedChanged="rdbtnpdf_CheckedChanged" /></td>
                            <td>
                                <asp:RadioButton ID="rdbtnExcel" runat="server" Text="Excel" AutoPostBack="True" OnCheckedChanged="rdbtnExcel_CheckedChanged" /></td>
                            <td>
                                <asp:Button ID="btnConvert" runat="server" Text="CONVERT" CssClass="btn btn-danger" OnClick="btnConvert_Click" /></td>
                        </tr>
                    </table>
                         <asp:MultiView ID="MultiView1" runat="server">
                            <asp:View ID="View1" runat="server">
                                    <asp:DataGrid ID="DataGrid1" runat="server" AllowPaging="True"  AutoGenerateColumns="True" PageSize="30"
                                    UseAccessibleHeader="true" GridLines="None" CssClass="table table-striped table-hover"  OnItemCommand="DataGrid1_ItemCommand"
                                        HorizontalAlign="Justify" OnPageIndexChanged="DataGrid1_PageIndexChanged">
                                    <FooterStyle BackColor="InactiveCaption"  ForeColor="White" />
                                    <PagerStyle  ForeColor="#4380B8" Mode="NumericPages" HorizontalAlign="Center"  Font-Size="16"/>
                                    <SelectedItemStyle BackColor="#E2DED6"  Font-Bold="True" ForeColor="#333333" />
                                    <Columns>
                                           <asp:ButtonColumn CommandName="btnEdit" HeaderText="Reply" Text="Area" DataTextField="PhoneNumber">    
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

