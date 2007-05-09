<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Login.ascx.cs" Inherits="Controls_Login" %>

<script language="JavaScript" type="text/javascript">
function checkIfEnter(evt) {
	var key = (evt.which) ? evt.which : event.keyCode;
		
	if (key == 13) {
		document.forms[0].submit();
		return false;
	} else {
		return true;
	}
}
</script>



<table>
	<tr>
		<td><strong>Username</strong></td>
		<td><asp:TextBox ID="txtUsername" Runat="server" Width="150"></asp:TextBox></td>
	</tr>
	<tr>
		<td><strong>Password</strong></td>
		<td><asp:TextBox ID="txtPassword" Runat="server" TextMode="Password" Width="150"></asp:TextBox></td>
	</tr>
	<tr>
		<td colspan="2"><asp:LinkButton ID="lnkSubmit" runat="server" Text="Submit" /></td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="2">If you aren't registered yet and you would like to, <a href="create_user.aspx">create a new user account</a>.</td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="2"><asp:Label ID="message" Runat="server" CssClass="warning"></asp:Label></td>
	</tr>
</table>