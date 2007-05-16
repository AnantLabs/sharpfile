using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Common;

/// <summary>
/// Summary description for SlogData
/// </summary>
public class SlogData : Data
{
	private SlogData()
	{
	}

	public static void InsertSlog(string title, string content, int userId)
	{
		SqlParameter[] parameters = getSqlParameters("@Content,@UserId,@Title",
			content, userId, title);

		NonQuery("usp_SlogInsert", parameters);
	}

	public static DataTable GetSlogs()
	{
		return Select("usp_SlogGet");
	}

	public static DataTable GetSlog(int id) {
		SqlParameter[] parameters = getSqlParameters("@Id", 
			id);

		return Select("usp_SlogGetSlog", parameters);
	}

	public static void DeleteSlog(int id) {
	}

	public static void UpdateSlog(int id, int userId, string title, string content) {
	}
}
