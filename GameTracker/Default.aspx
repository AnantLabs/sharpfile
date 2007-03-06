<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register TagPrefix="ajax" Namespace="MagicAjax.UI.Controls" Assembly="MagicAjax" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>GameTracker</title>
    <link rel="stylesheet" href="styles.css" type="text/css" />
    <script language="javascript" type="text/javascript">
		function Toggle(id) {
			if (this.document.getElementById(id).style.display == 'none') {
				this.document.getElementById(id).style.display = '';
			} else {
				this.document.getElementById(id).style.display = 'none';
			}
		}
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ajax:AjaxPanel ID="ajaxPanel" runat="server" AjaxCallConnection="asynchronous">
    
    TODO:<br />
    Matches should really be defined as having players and a date... maybe.
	<br />
	<br />
	 
		<table width="100%" class="newGame">
			<tr>
				<td>Filter the results...</td>
			</tr>
			<tr>
				<td>Game</td>
			</tr>
			<tr>
				<td><asp:DropDownList ID="GameFilterList" runat="server" /></td>
			</tr>
		</table>
		<br />
		
		<table width="100%" class="newGame">
			<tr>
				<td><a href="#" onclick="Toggle('newTournament')">Add new tournament...</a></td>
			</tr>
			<tr id="newTournament" style="display: none">
				<td>
					<table>
						<tr>
							<td>Tournament Name</td>
						</tr>
						<tr>
							<td><asp:TextBox ID="TournamentName" runat="server" /></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		
		<asp:Repeater ID="Matches" runat="server">
			<HeaderTemplate>
				<table width="100%">
			</HeaderTemplate>
			<ItemTemplate>
					<tr>
						<%# getTournamentResults(Container) %>					
						<%# getMatchResults(Container) %>
						<%# getGameResults(Container) %>
					</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>
	</ajax:AjaxPanel>
    </form>
</body>
</html>
