<%@ Page Title="ViewBalances" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewBalances.aspx.cs" Inherits="PEGBANK.ViewBalances" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Get Account Balance</h2>
     <asp:Label Text="" Visible="true" Font-Bold="true"
                    class="col-sm-12 control-label" ID="accbalanceError"  runat="server" />
     <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
            <asp:View ID="View1" runat="server">
                <div class="row">
                    <div class="col-lg-3">
                        <asp:TextBox ID="TextBox1" runat="server" placeholder ="Enter account number" style="padding: 3px;    max-height: 30px;" CssClass="form-control"></asp:TextBox>
                     </div>
                    
                    <div class="col-lg-3">
                        
                         <asp:Button ID="btnSearch" Text="VIEW BALANCE" class="btn btn-success btn-block form-control" Width="50%"
                         runat="server" OnClick="btnSearch_Click" /> 
                    </div>
                    <div class="col-lg-3">
                        
                         <asp:Button ID="Button3" Text="ALL BALANCES" class="btn btn-primary btn-block form-control" Width="50%"
                         runat="server" OnClick="Button3_Click" /> 
                    </div>
              
             </div>
                <br />
                <asp:GridView ID="GridView2" OnRowDataBound="GridView2_RowDataBound" runat="server" CssClass="table table-bordered table-condensed">

                </asp:GridView>
               

            </asp:View>
          
        </asp:MultiView>
    <br />

<%--    <h2>Get Account Statement</h2>
    <asp:Label Text="" Visible="true" Font-Bold="true"
                    class="col-sm-12 control-label" ID="statementError"  runat="server" />
    <asp:MultiView ID="MultiView2" runat="server" ActiveViewIndex="0">
        <asp:View ID="View2" runat="server"  >
            <div class ="row">
                <div class="col-lg-3">
                    <br />
                    <asp:TextBox ID="TextBox2" runat="server" placeholder ="Enter account number" style="padding: 3px;    max-height: 30px;" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-lg-3">
                    <asp:Label Text="START" Visible="true" Font-Bold="true" class="col-xl-3 col-sm-3 mb-3"  ID="Label2"  runat="server" />
                 <asp:TextBox  ID="DropDownList2" runat="server"  style="padding: 3px; max-height: 30px;"   CssClass="form-control" type="date">    </asp:TextBox>
              
                </div> 
                
                <div class="col-lg-3">
                    <asp:Label Text="END" Visible="true" Font-Bold="true" class="col-xl-3 col-sm-3 mb-3"  ID="Label3"  runat="server" Width="50%"/>
                 <asp:TextBox ID="DropDownList3" runat="server"  style="padding: 3px; max-height: 30px;"   CssClass="form-control" type="date">    </asp:TextBox>

                </div>  
                
                <div class="col-lg-3">
                    <br />
                    <asp:Button ID="Button1" runat="server" Text="VIEW STATEMENT" Width="50%" class="btn btn-success btn-block form-control" OnClick ="Button1_Click" />
        
                </div>     
            </div>
            
            <br />
            <asp:GridView ID="GridView1" runat="server"  OnRowDataBound="GridView1_RowDataBound" CssClass="table table-bordered table-condensed">

          </asp:GridView>
            <div class="row">
                <div class="col-lg-3">
                    <br />
                    <asp:Button ID="excel" runat="server" Text="EXPORT EXCEL" Width="50%" class="btn btn-success btn-block form-control" OnClick="excel_Click" />
        
                </div>   
            </div>
        </asp:View>
        
    </asp:MultiView>--%>

</asp:Content>


