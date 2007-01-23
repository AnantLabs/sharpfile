<%@ Page Language="C#" MasterPageFile="~/IndieLyrics/template.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" Title="IndieLyrics: Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">

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
		<td colspan="2">If you aren't registered yet and you would like to, <a href="create_user.aspx">create a new user account</a>.</td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="2"><asp:Label ID="message" Runat="server" CssClass="warning"></asp:Label></td>
	</tr>
</table>

</asp:Content>