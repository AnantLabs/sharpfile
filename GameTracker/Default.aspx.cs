using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections;

public partial class _Default : System.Web.UI.Page 
{
	private int _tournamentId;
	private int _matchId;
	private int _player1Id;
	private int _player2Id;

	private int tournamentId;
	private int matchId;
	private int player1Id;
	private int player2Id;
	private string player1;
	private string player2;
	private string tournamentName;
	private int player1Points;
	private int player2Points;
	private string dateTime;

	private string playerOptions = string.Empty;

	private WinnerGenerator winnerGenerator;

    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
		{
			foreach (DataRow row in Data.Select("usp_GetPlayers").Rows)
			{
				playerOptions += "<option value=" + row["Id"] + ">" + row["Name"] + "</option>";
			}
		}

		DataTable resultsTable = Data.GetRecords();		
		winnerGenerator = new WinnerGenerator(resultsTable);

		Matches.DataSource = resultsTable;
		Matches.DataBind();
    }

	private void getRepeaterItem(RepeaterItem item)
	{
		DataRowView row = (DataRowView)item.DataItem;

		tournamentName = row["TournamentName"] == Convert.DBNull ? "n/a" : row["TournamentName"].ToString();
		tournamentId = row["TournamentId"] == Convert.DBNull ? 0 : (int)row["TournamentId"];
		matchId = row["MatchId"] == Convert.DBNull ? 0 : (int)row["MatchId"];
		player1Id = row["Player1Id"] == Convert.DBNull ? 0 : (int)row["Player1Id"];
		player2Id = row["Player2Id"] == Convert.DBNull ? 0 : (int)row["Player2Id"];
		player1 = row["Player1"].ToString();
		player2 = row["Player2"].ToString();
		dateTime = row["DateTime"].ToString();
		player1Points = Convert.ToInt32(row["Player1Points"]);
		player2Points = Convert.ToInt32(row["Player2Points"]);
	}

	protected string getTournamentResults(RepeaterItem item)
	{
		string html = string.Empty;
		getRepeaterItem(item);

		if (tournamentId != _tournamentId)
		{
			string winner = winnerGenerator.GetTournamentWinner(tournamentId);

			html = string.Format(@"
	<td colspan=3>
		<table class=tournamentRow>
			<tr>
				<td><strong>Tournament: {0}</strong> ({1}) | <a href=""#"" onclick=""Toggle('newMatch_{2}')"">Add new match...</a></td>
			</tr>
			<tr id=""newMatch_{2}"" style=""display: none"">
				<td colspan=3>
					<table>
						<tr>
							<td>Player 1</td>
							<td>Player 2</td>
							<td>Date</td>
						</tr>
						<tr>
							<td><select name=""Player1List_{2}"" id=""Player1List_{3}"">{3}</select></td>
							<td><select name=""Player2List_{2}"" id=""Player2List_{3}"">{3}</select></td>
							<td><input type=text id=""Date_{2}"" value=""{4}"" /></td>
							<td><input type=button value=""Add"" /></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</td>
</tr>",
			  tournamentName,
			  winner,
			  tournamentId,
			  playerOptions,
			  DateTime.Now.ToString());

			_tournamentId = tournamentId;
		}

		return html;
	}

	protected string getMatchResults(RepeaterItem item)
	{
		string html = string.Empty;

		if (matchId != _matchId)
		{
			string record = winnerGenerator.GetMatchRecord(matchId);

			html = string.Format(@"
<tr>
	<td>&nbsp;</td>
	<td class=matchRow colspan=3>
		<table>
			<tr>
				<td>&nbsp;</td>
				<td colspan=2><strong>Match between {0} and {1}: {2}</strong> | <a href=""#"" onclick=""Toggle('newGame_{3}')"">Add new game...</a></td>
			</tr>
			<tr id=""newGame_{3}"" style=""display: none"">
				<td>&nbsp;</td>
				<td colspan=2>
					<table>
						<tr>
							<td>{0}'s Points</td>
							<td>{1}'s Points</td>
						</tr>
						<tr>
							<td><input type=text size=10></input></td>
							<td><input type=text size=10></input></td>
							<td><input type=button value=""Add"" /></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</td>
</tr>",
		player1,
		player2,
		record,
		matchId,
		playerOptions,
		DateTime.Now.ToString());

			_matchId = matchId;
			_player1Id = player1Id;
			_player2Id = player2Id;
		}

		return html;
	}

	public string getGameResults(RepeaterItem item)
	{
		return string.Format(@"
<tr>
	<td>&nbsp;</td>
	<td>&nbsp;</td>
	<td>
		<table>
			<tr>
				<td>&nbsp;</td>
				<td><strong>{0}</strong></td>
				<td><strong>{1}</strong></td>
			</tr>
			<tr>
				<td>{2}</td>
				<td align=""right"">{3}</td>
				<td align=""right"">{4}</td>
			</tr>
		</table>
	</td>",
		  player1,
		  player2,
		  dateTime,
		  player1Points,
		  player2Points);
	}
}
