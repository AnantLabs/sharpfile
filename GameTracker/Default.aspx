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
		<table width="100%" class="newGame">
			<tr>
				<td>Filter the results...</td>
			</tr>
			<tr>
				<td>Player</td>
			</tr>
			<tr>
				<td>blob</td>
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
		
		<asp:Repeater ID="Matches_old" runat="server" Visible="false">
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
		
		<asp:Repeater ID="Tournaments" runat="server">
			<HeaderTemplate>
				<table width="100%">
			</HeaderTemplate>
			<ItemTemplate>
				<input id="TournamentId" runat="server" type="hidden" value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
				<tr>
					<td>
						<table class="tournamentRow" width="100%">
							<tr>
								<td><strong>Tournament: <%# DataBinder.Eval(Container.DataItem, "Name") %></strong> (<%# "winner" %>) | <a href="#" onclick="Toggle('newMatch_<%# DataBinder.Eval(Container.DataItem, "Id") %>')">Add new match/game...</a></td>
							</tr>
							<tr id="newMatch_<%# DataBinder.Eval(Container.DataItem, "Id") %>" style="display: none">
								<td>
									<table>
										<tr>
											<td>Player 1</td>
											<td>Player 2</td>
											<td>Date</td>
										</tr>
										<tr>
											<td><asp:DropDownList ID="Player1List" runat="server" /></td>
											<td><asp:DropDownList ID="Player2List" runat="server" /></td>
											<td><asp:TextBox ID="Date" Text='<%# DateTime.Now.ToString() %>' runat="server" /></td>
										</tr>
										<tr>
											<td><asp:TextBox ID="Player1Points" Width="30" runat="server" /></td>
											<td><asp:TextBox ID="Player2Points" Width="30" runat="server" /></td>
											<td><asp:LinkButton ID="AddMatch" Text="Add" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' OnClick="AddMatch_OnClick" runat="server" /></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Repeater ID="Matches" runat="server">
							<HeaderTemplate>
								<table width="100%">
							</HeaderTemplate>
							<ItemTemplate>
								<input id="MatchId" runat="server" type="hidden" value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
								<tr>
									<td>&nbsp;</td>
									<td class="matchRow" width="100%">
										<table>
											<tr>
												<td><strong>Match between <%# DataBinder.Eval(Container.DataItem, "Player1") %> and <%# DataBinder.Eval(Container.DataItem, "Player2") %>: <%# "record" %></strong> | <a href="#" onclick="Toggle('newGame_<%# DataBinder.Eval(Container.DataItem, "Id") %>')">Add new game...</a></td>
											</tr>
											<tr id="newGame_<%# DataBinder.Eval(Container.DataItem, "Id") %>" style="display: none">
												<td>
													<table>
														<tr>
															<td><%# DataBinder.Eval(Container.DataItem, "Player1") %>'s Points</td>
															<td><%# DataBinder.Eval(Container.DataItem, "Player2") %>'s Points</td>
														</tr>
														<tr>
															<td><asp:TextBox ID="Player1Points" Width="30" runat="server" /></td>
															<td><asp:TextBox ID="Player2Points" Width="30" runat="server" /></td>
															<td><asp:LinkButton ID="AddGame" Text="Add" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' OnClick="AddGame_OnClick" runat="server" /></td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<table>
											<tr>
												<td>&nbsp;</td>
												<td>&nbsp;</td>
												<td>
													<table>
														<tr>
															<td><strong><%# DataBinder.Eval(Container.DataItem, "DateTime") %></strong></td>
															<td><strong><%# DataBinder.Eval(Container.DataItem, "Player1") %></strong></td>
															<td><strong><%# DataBinder.Eval(Container.DataItem, "Player2") %></strong></td>
														</tr>
															<asp:Repeater ID="Games" runat="server">
																<HeaderTemplate>
																</HeaderTemplate>
																<ItemTemplate>
																	<input id="GameId" runat="server" type="hidden" value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
																	<tr>
																		<td>&nbsp;</td>
																		<td align="right"><%# DataBinder.Eval(Container.DataItem, "Player1Points") %></td>
																		<td align="right"><%# DataBinder.Eval(Container.DataItem, "Player2Points") %></td>
																	</tr>									
																</ItemTemplate>
																<FooterTemplate>
																</FooterTemplate>
															</asp:Repeater>
													</table>
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</ItemTemplate>
							<FooterTemplate>
								</table>
							</FooterTemplate>
						</asp:Repeater>
					</td>
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
