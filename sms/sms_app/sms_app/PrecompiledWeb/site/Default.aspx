<%@ page language="C#" autoeventwireup="true" inherits="_Default, App_Web_3v2sk105" enableeventvalidation="false" culture="auto" uiculture="auto" %>


 <%@ Register 
 Assembly="AjaxControlToolkit" 
 Namespace="AjaxControlToolkit" 
 TagPrefix="ajaxToolkit" %>
 <%@ Import
  Namespace="System.Threading" %>
  
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

  <head>

    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no"/>
	<meta name="description" content=""/>
    <meta name="author" content=""/>
    <title>SMS PORTAL</title>
   <link rel="icon" href="Images/favicon.png"/>
	

    <!-- Bootstrap core CSS -->
    <link href="vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css"/>

    <!-- Custom fonts for this template -->
    <link href="vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css"/>

    <!-- Plugin CSS -->
    <link href="vendor/datatables/dataTables.bootstrap4.css" rel="stylesheet" type="text/css"/>

    <!-- Custom styles for this template -->
    <link href="css/sb-admin.css" rel="stylesheet" type="text/css"/>

  </head>

<body style="background-color: #E2E2E2;">
<form id="form1" runat="server">
    <div class="container col-md-4 col-md-offset-4 col-sm-6 col-sm-offset-3 col-xs-10 col-xs-offset-1 ">
                <div class="row " style="padding-top:100px;">
            <center><asp:Label ID="lblmsg" runat="server" ForeColor="Red" Text="."></asp:Label></center>
                <div class="modal-content">
                
                   
                     <div class="modal-header">
							<img class="modal-title" style="max-width: 100%;" src="Images/peglogo.png" alt="logo"/>
						</div>
						   <asp:MultiView ID="MultiView1" runat="server">
                                <asp:View ID="View2" runat="server">
                                
                                    <div class="modal-body">
                                        <center><h4>PEGASUS SMS PORTAL</h4>
                                        <hr/>
                                        </center>
                                        
								        <label>Email</label>
                                        <div class="form-group input-group">
									        <span class="input-group-addon"><i class="fa fa-envelope"  ></i></span>
									        <asp:TextBox ID="txtUsername" placeholder="Username" runat="server" CssClass="form-control " onblur="Change(this, event)" onfocus="Change(this, event)"></asp:TextBox>
								        </div><br/>
								        <label>Password</label>
								        <div class="form-group input-group">
									        <span class="input-group-addon"><i class="fa fa-lock"  ></i></span>
									       <asp:TextBox ID="txtpassword" placeholder="password" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                        </div>
                                    </div>
                                     <div class="modal-header">
							
							        <asp:Button ID="btnlogin" runat="server" CssClass="btn btn-success  col-md-4 pull-left" Text="Login" OnClick="btnlogin_Click" />
                                    <asp:Button ID="btnForgot" runat="server" CssClass="btn btn-blue  col-md-4 pull-right" Text="Forgot Password" OnClick="btnlogin_Forgot" />
							        
						            </div>
                                
			                    <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="#C00000"></asp:Label>
                                </asp:View>

                                 <asp:View ID="View1" runat="server">
                                    <div class="modal-body">
                                         <center><h4>RESET PASSWORD</h4>
                                        <hr/>
                                        </center>
								        <label>New Password</label>
                                        <div class="form-group input-group">
									        <span class="input-group-addon"><i class="fa fa-lock"  ></i></span>
									       <asp:TextBox ID="txtResetPasswd" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
								        </div><br/>
								        <label>Confirm Password</label>
								        <div class="form-group input-group">
									        <span class="input-group-addon"><i class="fa fa-lock"  ></i></span>
									       <asp:TextBox ID="txtResetConfirm" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="modal-header">
                                     
                                     <asp:Button ID="btnchange" runat="server" CssClass="btn btn-success col-md-4 pull-left"  OnClick="btnchange_Click" Text="Save" />
                                     <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-danger col-md-4 pull-right"  OnClick="btnCancel_Click" Text="Cancel"  />
				                       
                                    </div>
                                    
                                </asp:View>

                                <asp:View ID="View3" runat="server">
                                    <div class="modal-body">
                                         <center><h4>FORGOT PASSWORD</h4>
                                        <hr/>
                                        </center>
								       <%-- <label>Username</label>
                                        <div class="form-group input-group">
									        <span class="input-group-addon"><i class="fa fa-user"  ></i></span>
									       <asp:TextBox ID="txtUser" runat="server" CssClass="form-control"></asp:TextBox>
								        </div>--%><br/>
								        <label>Email</label>
								        <div class="form-group input-group">
									        <span class="input-group-addon"><i class="fa fa-envelope"  ></i></span>
									       <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="modal-header">
                                      <asp:Button ID="btnCancel2" runat="server" CssClass="btn btn-danger col-md-4 pull-left"  OnClick="btnCancel_Click" Text="Cancel"  />
                                     <asp:Button ID="btnReset" runat="server" CssClass="btn btn-success col-md-4 pull-right"  OnClick="btnpassword_Reset" Text="RESET" />
				                       
                                    </div>
                                    
                                </asp:View>
                            </asp:MultiView>
					</div><!-- /.modal-content -->
                       <asp:Label ID="lblUsercode" runat="server" Text="0" Visible="False"></asp:Label>     
				</div>

    </div>
	<div id="footer-sec" style="text-align:center; margin-top: 20px;">
        &copy; 2018 - <%=DateTime.Now.ToString("yyyy") %> |  PEGASUS TECHNOLOGIES LTD</a>
    </div>
</form>
 <!-- Bootstrap core JavaScript -->
    <script src="vendor/jquery/jquery.min.js" type="text/javascript"></script>
    <script src="vendor/popper/popper.min.js" type="text/javascript"></script>
    <script src="vendor/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>

    <!-- Plugin JavaScript -->
    <script src="vendor/jquery-easing/jquery.easing.min.js" type="text/javascript"></script>
    <script src="vendor/chart.js/Chart.min.js" type="text/javascript"></script>
    <script src="vendor/datatables/jquery.dataTables.js" type="text/javascript"></script>
    <script src="vendor/datatables/dataTables.bootstrap4.js" type="text/javascript"></script>

    <!-- Custom scripts for this template -->
    <script src="js/sb-admin.min.js" type="text/javascript"></script>
</body>
</html>
