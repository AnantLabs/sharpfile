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
}
