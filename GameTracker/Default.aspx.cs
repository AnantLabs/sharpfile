using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page 
{
	private DataTable playerTable;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			refresh();
		}
	}

	void Tournaments_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		RepeaterItem item = e.Item;

		if (item.ItemType == ListItemType.AlternatingItem ||
			item.ItemType == ListItemType.Item)
		{
			DropDownList playerList = (DropDownList)item.FindControl("Player1List");
			playerList.DataSource = playerTable;
			playerList.DataTextField = "Name";
			playerList.DataValueField = "Id";
			playerList.DataBind();
			playerList.SelectedIndex = 0;

			playerList = (DropDownList)item.FindControl("Player2List");
			playerList.DataSource = playerTable;
			playerList.DataTextField = "Name";
			playerList.DataValueField = "Id";
			playerList.DataBind();
			playerList.SelectedIndex = 0;

			int tournamentId = int.Parse(((HtmlInputHidden)item.FindControl("TournamentId")).Value);

			Repeater Matches = (Repeater)item.FindControl("Matches");
			Matches.ItemDataBound += new RepeaterItemEventHandler(Matches_ItemDataBound);
			Matches.DataSource = Data.Select("usp_GetMatches", "@TournamentId", new object[] { tournamentId });
			Matches.DataBind();
		}
	}

	void Matches_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		RepeaterItem item = e.Item;

		if (item.ItemType == ListItemType.AlternatingItem ||
			item.ItemType == ListItemType.Item)
		{
			int matchId = int.Parse(((HtmlInputHidden)item.FindControl("MatchId")).Value);

			((Literal)item.FindControl("Winner")).Text = WinnerGenerator.GetMatchRecord(matchId);
			
			Repeater games = (Repeater)item.FindControl("Games");
			games.DataSource = Data.Select("usp_GetGames", "@MatchId", new object[] { matchId });
			games.DataBind();
		}
	}
	
	protected void AddTournament_OnClick(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(TournamentName.Text))
		{
			Data.NonQuery("usp_InsertTournament",
			              "@Name",
			              new object[] { TournamentName.Text });

			refresh();
		}
	}

	protected void AddMatch_OnClick(object sender, EventArgs e)
	{
		int tournamentId = int.Parse(((LinkButton)sender).CommandArgument);
		string player1;
		string player2;
		string dateTime;
		string player1Points;
		string player2Points;

		foreach (Control tournamentItem in Tournaments.Controls)
		{
			if (((HtmlInputHidden)tournamentItem.FindControl("TournamentId")).Value == tournamentId.ToString())
			{
				player1 = ((DropDownList)tournamentItem.FindControl("Player1List")).SelectedValue;
				player2 = ((DropDownList)tournamentItem.FindControl("Player2List")).SelectedValue;

				if (player1 == player2)
				{
					// throw some exception.
				}

				dateTime = ((TextBox)tournamentItem.FindControl("Date")).Text;

				if (string.IsNullOrEmpty(dateTime))
				{
					dateTime = DateTime.Now.ToString();
				}

				player1Points = ((TextBox)tournamentItem.FindControl("Player1Points")).Text;
				player2Points = ((TextBox)tournamentItem.FindControl("Player2Points")).Text;

				if (string.IsNullOrEmpty(player1Points))
				{
					player1Points = "0";
				}

				if (string.IsNullOrEmpty(player2Points))
				{
					player2Points = "0";
				}

				Data.NonQuery("usp_InsertMatch",
					"@TournamentId,@DateTime,@Player1,@Player1Points,@Player2,@Player2Points",
					tournamentId,
					dateTime,
					player1,
					player1Points,
					player2,
					player2Points);

				break;
			}
		}

		refresh();
	}

	protected void AddGame_OnClick(object sender, EventArgs e)
	{
		int matchId = int.Parse(((LinkButton)sender).CommandArgument);
		string player1;
		string player2;
		string player1Points;
		string player2Points;

		foreach (Control tournamentItem in Tournaments.Controls)
		{
			foreach (Control matchItem in tournamentItem.FindControl("Matches").Controls)
			{
				if (((HtmlInputHidden)matchItem.FindControl("MatchId")).Value == matchId.ToString())
				{
					player1 = ((HtmlInputHidden)matchItem.FindControl("Player1")).Value;
					player2 = ((HtmlInputHidden)matchItem.FindControl("Player2")).Value;

					if (player1 == player2)
					{
						// throw some exception.
					}

					player1Points = ((TextBox)matchItem.FindControl("Player1Points")).Text;
					player2Points = ((TextBox)matchItem.FindControl("Player2Points")).Text;

					if (string.IsNullOrEmpty(player1Points))
					{
						player1Points = "0";
					}

					if (string.IsNullOrEmpty(player2Points))
					{
						player2Points = "0";
					}

					Data.NonQuery("usp_InsertGame",
						"@MatchId,@Player1,@Player1Points,@Player2,@Player2Points",
						matchId,
						player1,
						player1Points,
						player2,
						player2Points);

					break;

				}
			}
		}

		refresh();
	}

	private void refresh()
	{
		playerTable = Data.Select("usp_GetPlayers");

		Tournaments.ItemDataBound += new RepeaterItemEventHandler(Tournaments_ItemDataBound);
		Tournaments.DataSource = Data.Select("usp_GetTournaments");
		Tournaments.DataBind();
	}
}
