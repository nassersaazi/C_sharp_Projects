<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterUser.aspx.cs" Inherits="PEGBANK.RegisterUser" %>



<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2>Bank App</h2>
    

    <div class="form-horizontal">
        <h4>Register Here</h4>
        <hr />
        
        <div class="form-group">
            <asp:Label runat="server"  CssClass="col-md-2 control-label">First Name</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="txtName" class="form-control p_input" /> 
                
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server"  CssClass="col-md-2 control-label">Last Name</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="TextBox1" class="form-control p_input" /> 
                
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server"  CssClass="col-md-2 control-label">Phone</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="TextBox2" class="form-control p_input" /> 
                
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server"  CssClass="col-md-2 control-label">Email</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="txtEmail"  class="form-control p_input" />
               
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button ID ="CreateUserButton" runat="server" OnClick="CreateUser_Click" Text="Register" CssClass="btn btn-default" />
            </div>
        </div>
        <asp:Label Text="" Visible="true" Font-Bold="true"
                    class="col-sm-12 control-label" ID="registerError"  runat="server" />
    </div>
</asp:Content>
