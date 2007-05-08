<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Admin_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Login</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		Username: <asp:TextBox ID="txtUsername" runat="server" /><br />
		Password: <asp:TextBox ID="txtPassword" runat="server" /><br />
		
		<asp:Button ID="btnSubmit" runat="server" Text="Submit" />		
    </div>
    </form>
</body>
</html>
