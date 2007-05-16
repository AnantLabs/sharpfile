<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login_non_template.aspx.cs" Inherits="login_non_template" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <script language="JavaScript" type="text/javascript">
	function SetMultiTaskType(action) {
		document.forms[0].MultiTaskType.value = action;
	}
	
	function checkIfEnter(evt) {
		var key = (evt.which) ? evt.which : event.keyCode;
		
		if (key == 13) {
			SetMultiTaskType('submit');
			document.forms[0].submit();
			return false;
		} else {
			return true;
		}
	}
</script>

<input type="hidden" name="MultiTaskType" />

Login to the site here:
<table>
	<tr>
		<td><strong>Username</strong></td>
		<td><asp:TextBox ID="user" Runat="server" Width="150"></asp:TextBox></td>
	</tr>
	<tr>
		<td><strong>Password</strong></td>
		<td><asp:TextBox ID="password" Runat="server" TextMode="Password" Width="150" onKeyDown="checkIfEnter(event);"></asp:TextBox></td>
	</tr>
	<tr>
		<td colspan="2"><a href="#" onclick="SetMultiTaskType('submit'); document.forms[0].submit(); return false;">Submit</a></td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="2"><asp:Label ID="message" Runat="server" CssClass="warning"></asp:Label></td>
	</tr>
</table>
    </div>
    </form>
</body>
</html>
