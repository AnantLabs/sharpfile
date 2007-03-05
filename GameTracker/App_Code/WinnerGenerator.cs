using System;
using System.Data;
using System.Collections;

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
		return computeWinner("tournamentId = " + tournamentId);
	}

	public string GetMatchRecord(int matchId)
	{
		string filter = "MatchId = " + matchId;

		return computeRecord(filter);
	}

	public string GetPlayerRecord(int player1Id, int player2Id)
	{
		string filter = "Player1Id = " + player1Id + " AND Player2Id = " + player2Id + " AND MatchId IS NULL AND TournamentId IS NULL";

		return computeRecord(filter);
	}

	private string computeRecord(string filter)
	{
		int player1Wins = 0;
		int player2Wins = 0;

		foreach (DataRow row in resultsTable.Select(filter))
		{
			int player1Points = Convert.ToInt32(row["Player1Points"]);
			int player2Points = Convert.ToInt32(row["Player2Points"]);

			if (player1Points > player2Points)
			{
				player1Wins++;
			}
			else
			{
				player2Wins++;
			}
		}

		return string.Format("{0} - {1}",
			player1Wins,
			player2Wins);
	}

	private string computeWinner(string filter)
	{
		Hashtable recordHash = new Hashtable();
		Hashtable scoreHash = new Hashtable();
		string winner = "n/a";

		foreach (DataRow row in resultsTable.Select(filter))
		{
			string tempWinner;
			string player1 = row["Player1"].ToString();
			string player2 = row["Player2"].ToString();
			int player1Points = Convert.ToInt32(row["Player1Points"]);
			int player2Points = Convert.ToInt32(row["Player2Points"]);

			if (player1Points > player2Points)
			{
				tempWinner = player1;
			}
			else
			{
				tempWinner = player2;
			}

			if (!recordHash.ContainsKey(tempWinner))
			{
				recordHash.Add(tempWinner, 1);
			}
			else
			{
				recordHash[tempWinner] = Convert.ToInt32(recordHash[tempWinner]) + 1;
			}

			if (!scoreHash.ContainsKey(player1))
			{
				scoreHash.Add(player1, player1Points);
			}
			else
			{
				scoreHash[player1] = Convert.ToInt32(scoreHash[player1]) + player1Points;
			}

			if (!scoreHash.ContainsKey(player2))
			{
				scoreHash.Add(player2, player2Points);
			}
			else
			{
				scoreHash[player2] = Convert.ToInt32(scoreHash[player2]) + player2Points;
			}
		}

		int maxWins = -1;
		bool sameRecord = false;

		foreach (string player in recordHash.Keys)
		{
			int playerWins = Convert.ToInt32(recordHash[player]);

			if (playerWins > maxWins)
			{
				winner = player;
				maxWins = playerWins;
			}
			else if (Convert.ToInt32(recordHash[player]) == maxWins)
			{
				sameRecord = true;
			}
		}

		if (sameRecord)
		{
			int maxScore = -1;

			foreach (string player in scoreHash.Keys)
			{
				int playerScore = Convert.ToInt32(scoreHash[player]);

				if (playerScore > maxScore)
				{
					winner = player;
					maxScore = playerScore;
				}
				else if (playerScore == maxScore)
				{
					winner = "There has been a tie and I don't want to figure out the results.";
				}
			}
		}

		return winner;
	}
}
