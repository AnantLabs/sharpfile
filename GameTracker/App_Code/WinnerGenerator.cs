using System;
using System.Data;

/// <summary>
/// Summary description for WinnerWizard
/// </summary>
public class WinnerGenerator
{
	private DataTable resultsTable;

	public WinnerGenerator(DataTable resultsTable)
	{
		this.resultsTable = resultsTable;
	}

	public string GetTournamentWinner(int tournamentId)
	{
		return "blob";
	}

	public string GetMatchRecord(int matchId)
	{
		return "0 - 0";
	}

	public string GetPlayerRecord(int player1Id, int player2Id)
	{
		return "0 - 0";
	}
}
