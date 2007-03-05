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

	private WinnerGenerator winnerGenerator;

    protected void Page_Load(object sender, EventArgs e)
    {
		Date.Text = DateTime.Now.ToString();

		DataTable resultsTable = Data.GetRecords();		
		winnerGenerator = new WinnerGenerator(resultsTable);

		Matches.DataSource = resultsTable;
		Matches.DataBind();

		Player1List.DataSource = Data.Select("usp_GetPlayers");
		Player1List.DataTextField = "Name";
		Player1List.DataValueField = "Id";
		Player1List.DataBind();

		Player2List.DataSource = Data.Select("usp_GetPlayers");
		Player2List.DataTextField = "Name";
		Player2List.DataValueField = "Id";
		Player2List.DataBind();

		MatchList.DataSource = Data.Select("usp_GetMatches");
		MatchList.DataTextField = "Id";
		MatchList.DataValueField = "Id";
		MatchList.DataBind();
		MatchList.Items.Insert(0, "");

		TournamentList.DataSource = Data.Select("usp_GetTournaments");
		TournamentList.DataTextField = "Name";
		TournamentList.DataValueField = "Id";
		TournamentList.DataBind();
		TournamentList.Items.Insert(0, "");

		DataTable gameNameTable = Data.Select("usp_GetGameNames");
		GameList.DataSource = gameNameTable;
		GameList.DataTextField = "Name";
		GameList.DataValueField = "Id";
		GameList.DataBind();

		GameFilterList.DataSource = gameNameTable;
		GameFilterList.DataTextField = "Name";
		GameFilterList.DataValueField = "Id";
		GameFilterList.DataBind();
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
	<td colspan=3 class=tournamentRow><strong>{0}</strong> ({1})</td>
</tr>",
	  tournamentName,
	  winner);

			_tournamentId = tournamentId;
		}
		else if (tournamentName.Equals("n/a") && item.ItemIndex == 0)
		{
			html = string.Format(@"
	<td colspan=3 class=tournamentRow><strong>{0}</strong></td>
</tr>",
	  tournamentName);
		}

		return html;
	}

	protected string getMatchResults(RepeaterItem item)
	{
		string html = string.Empty;

		if (matchId != _matchId ||
			(player1Id != _player1Id ||
			player2Id != _player2Id))
		{
			string record = string.Empty;

			if (matchId != _matchId)
			{
				record = winnerGenerator.GetMatchRecord(matchId);
			}
			else if (player1Id != _player1Id ||
					player2Id != _player2Id)
			{
				record = winnerGenerator.GetPlayerRecord(player1Id, player2Id);
			}

			html = string.Format(@"
<tr>
	<td>&nbsp;</td>
	<td colspan=2 class=matchRow>Record: {0}</td>
</tr>",
	  record);

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
			<tr class=gameRow>
				<td>DateTime</td>
				<td>Player 1</td>
				<td>Player 2</td>
			</tr>
			<tr>
				<td>{0}</td>
				<td>{1} ({2})</td>
				<td>{3} ({4})</td>
			</tr>
		</table>
	</td>",
		  dateTime,
		  player1,
		  player1Points,
		  player2,
		  player2Points);
	}
}
