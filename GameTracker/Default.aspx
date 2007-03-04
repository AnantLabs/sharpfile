<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>GameTracker</title>
     <link rel="stylesheet" href="styles.css" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<strong>Game</strong>
		<asp:DropDownList ID="GamesList" runat="server">
			<asp:ListItem Text="Ping Pong" Value="1" />
		</asp:DropDownList>
		<br />
		
		<br />
		<table class="newGame">
			<tr>
				<td>Add new game...</td>
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
    </div>
    </form>
</body>
</html>
