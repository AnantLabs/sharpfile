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
	private DataTable playerTable;

    protected void Page_Load(object sender, EventArgs e)
    {
		playerTable = Data.Select("usp_GetPlayers");

		Tournaments.ItemDataBound += new RepeaterItemEventHandler(Tournaments_ItemDataBound);

		Tournaments.DataSource = Data.Select("usp_GetTournaments");
		Tournaments.DataBind();
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

			Repeater Games = (Repeater)item.FindControl("Games");
			Games.DataSource = Data.Select("usp_GetGames", "@MatchId", new object[] { matchId });
			Games.DataBind();
		}
	}

	protected void AddMatch_OnClick(object sender, EventArgs e)
	{
		int tournamentId = int.Parse(((LinkButton)sender).CommandArgument);

		//Response.Redirect(Request.Path);
	}

	protected void AddGame_OnClick(object sender, EventArgs e)
	{
		int matchId = int.Parse(((LinkButton)sender).CommandArgument);

		//Response.Redirect(Request.Path);
	}
}
