<%@ Page Language="C#" MasterPageFile="~/IndieLyrics/template.master" AutoEventWireup="true" CodeFile="create_user.aspx.cs" Inherits="create_user" Title="IndieLyrics: Create New User" %>
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
		<td><asp:TextBox ID="UserName" Runat="server" Width="150"></asp:TextBox></td>
		<td><strong>*</strong></td>
	</tr>
	<tr>
		<td><strong>Email</strong></td>
		<td valign="top"><asp:TextBox ID="Email" Runat="server" Width="150"></asp:TextBox></td>
		<td><strong>*</strong></td>
	</tr>
	<tr>
		<td><strong>Password</strong></td>
		<td><asp:TextBox ID="Pass" Runat="server" TextMode="Password" Width="150" onKeyDown="checkIfEnter(event);"></asp:TextBox></td>
		<td><strong>*</strong></td>
	</tr>
	<tr>
		<td colspan="3"><a href="" onclick="SetMultiTaskType('submit'); document.forms[0].submit(); return false;">Submit</a></td>
	</tr>
	<tr>
		<td colspan="3"><asp:Label ID="CreateError" Runat="server" CssClass="warning"></asp:Label></td>
	</tr>
</table>

</asp:Content>

