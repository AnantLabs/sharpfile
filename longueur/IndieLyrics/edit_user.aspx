<%@ Page Language="C#" MasterPageFile="~/IndieLyrics/template.master" AutoEventWireup="true" CodeFile="edit_user.aspx.cs" Inherits="edit_user" Title="IndieLyrics: Edit User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">

<script language="JavaScript" type="text/javascript">
	function SetMultiTaskType(action) {
		document.forms[0].MultiTaskType.value = action;
	}
	
	function checkIfEnter(evt) {
		var key = (evt.which) ? evt.which : event.keyCode;
		
		if (key == 13) {
			SetMultiTaskType('save');
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
		<td colspan="2"><asp:Label ID="UserName" Runat="server"></asp:Label></td>
	</tr>
	<tr>
		<td><strong>Email</strong></td>
		<td colspan="2"><asp:TextBox ID="Email" Runat="server"></asp:TextBox></td>
	</tr>
	<tr>
		<td><strong>New Password</strong></td>
		<td colspan="2"><asp:TextBox ID="NewPassword" runat="server" TextMode="Password" Width="150"></asp:TextBox></td>
	</tr>
	<tr>
		<td><strong>New Password (to confirm)</strong></td>
		<td colspan="2"><asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password" Width="150"></asp:TextBox></td>
	</tr>
	<tr>
		<td colspan="3">&nbsp;</td>
	</tr>
	<tr>
		<td><strong>Current Password</strong></td>
		<td colspan="2"><asp:TextBox ID="CurrentPassword" Runat="server" TextMode="Password" Width="150" onkeydown="checkIfEnter(event);"></asp:TextBox></td>
	</tr>
	<tr> 
		<td colspan="3"><a href="#" onclick="SetMultiTaskType('save'); document.forms[0].submit(); return false;">Save Updated Info</a></td>
	</tr>
	<tr>
		<td colspan="3"><a href="#" onclick="SetMultiTaskType('cancel'); document.forms[0].submit(); return false;">Cancel</a></td>
	</tr>
	<tr>
		<td colspan="3">&nbsp;</td>
	</tr>
	<tr>
		<td colspan="3">Do you want to delete your user? (Please note that this will make any of your contributed quotes, comments, ratings, etc. be associated with an anonymous user). <a href="" onclick="if (confirm('Are you sure you want to delete this user?')) { SetMultiTaskType('delete'); document.forms[0].submit(); } return false;">Delete user</a> anyway.</td>
	</tr>
	<tr>
		<td colspan="3"><asp:Label ID="Message" Runat="server" CssClass="warning"></asp:Label></td>
	</tr>
</table>

</asp:Content>

