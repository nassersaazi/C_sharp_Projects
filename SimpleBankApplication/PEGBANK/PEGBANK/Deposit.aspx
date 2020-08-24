<%@ Page Title="Deposit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Deposit.aspx.cs" Inherits="PEGBANK.Deposit" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>DEPOSITS</h2>

    <div class="row ">


        <div class="col-xl-3 col-sm-6 mb-3">
             
            <div>
                <asp:Label Text="ACCOUNT NUMBER" Visible="true" Font-Bold="true" class="col-sm-12 control-label" ID="Label1" runat="server" />
                <asp:TextBox ID="TextBox1" runat="server" Style="padding: 3px; max-height: 30px;" CssClass="form-control"></asp:TextBox>
              
              </div>  
       
        <asp:Label ID="Label2" runat="server" EnableViewState="False"></asp:Label>  
            <div>
                <asp:Label Text="AMOUNT" Visible="true" Font-Bold="true"
                    class="col-sm-12 control-label" ID="Label3" runat="server" />
                <asp:TextBox ID="txtAmount" runat="server" Style="padding: 3px; max-height: 30px; " CssClass="form-control"></asp:TextBox>

            </div>

            <p></p>
                <asp:Button ID="btnSubmit" Text="SUBMIT" class="btn btn-success btn-block " width="50%"
                    runat="server" OnClick="btnSubmit_Click" />

           
        </div>
        <asp:Label Text="" Visible="true" Font-Bold="true" class="col-sm-12 control-label" ID="depositError"  runat="server" />

        
    </div>


</asp:Content>

