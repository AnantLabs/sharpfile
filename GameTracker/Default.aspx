<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ Register TagPrefix="ajax" Namespace="MagicAjax.UI.Controls" Assembly="MagicAjax" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>GameTracker</title>
     <link rel="stylesheet" href="styles.css" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <ajax:AjaxPanel ID="ajaxPanel" runat="server" AjaxCallConnection="asynchronous">
		<table width="100%" class="newGame">
			<tr>
				<td colspan=6>Add new game...</td>
			</tr>
			<tr>
				<td>Game</td>
				<td>Player 1</td>
				<td>Player 2</td>
				<td>Date</td>
				<td>Tournament</td>
				<td>Match</td>
			</tr>
			<tr>
				<td><asp:DropDownList ID="GameList" runat="server" /></td>
				<td>Name: <asp:DropDownList ID="Player1List" runat="server" /></td>
				<td>Name: <asp:DropDownList ID="Player2List" runat="server" /></td>
				<td><asp:TextBox runat="server" ID="Date" /></td>
				<td><asp:DropDownList ID="TournamentList" runat="server" /></td>
				<td><asp:DropDownList ID="MatchList" runat="server" /></td>
			</tr>
			<tr>
				<td></td>
				<td>Points: <asp:TextBox ID="Player1Points" runat="server" Width="30" /></td>
				<td>Points: <asp:TextBox ID="Player2Points" runat="server" Width="30" /></td>
			</tr>
		</table>
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
