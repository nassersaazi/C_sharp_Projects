<%@ Page Title="Withdraw" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Withdraw.aspx.cs" Inherits="PEGBANK.Withdraw" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Withdraw funds here</h2>

    <div class="row">
         
         <div class="col-xl-3 col-sm-6 mb-3">  
              <asp:Label Text="" Visible="true" Font-Bold="true"
                    class="col-sm-12 control-label" ID="withrawError"  runat="server" />        
            <div >
             <asp:Label Text="ACCOUNT NUMBER" Visible="true" Font-Bold="true" class="col-sm-12 control-label"  ID="Label1"  runat="server" />
               
                <asp:TextBox ID="TextBox1" runat="server" style="padding: 3px;    max-height: 30px;" CssClass="form-control"></asp:TextBox>
                
                                   
             </div>  
             
             <div >
             <asp:Label Text="AMOUNT" Visible="true" Font-Bold="true"
              class="col-sm-12 control-label"  ID="Label2"  runat="server" />
                 <asp:TextBox ID="txtAmount" runat="server"  style="padding: 3px;    max-height: 30px;" CssClass="form-control"></asp:TextBox>
                                   
             </div> 

             <p></p>
             <div>
               <asp:Button ID="btnSubmit" Text="SUBMIT" class="btn btn-success btn-sm " width="50%"
                            runat="server" onclick="btnSubmit_Click"  />  
                         
             </div>    
                      
        </div>
       
         
    
         <div class="col-xl-3 col-sm-6 mb-3">        
                 
        </div>
     </div> 
  
    

</asp:Content>

