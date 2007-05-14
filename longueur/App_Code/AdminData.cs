using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Common;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for AdminData
/// </summary>
public class AdminData : Data
{
	private AdminData()
	{
	}

	public static DataTable GetErrorLog()
	{
		return Select("usp_ErrorLogGet");
	}

	public static DataTable GetDownloads()
	{
		return Select("usp_DownloadGet");
	}

	public static SiteUser GetUser(int userId)
	{
		return getUserAdmin(userId);
	}

	public static void UpdateUser(int id, string name, string email, UserType type) {
		UpdateUser(id, name, email, string.Empty, type);
	}

	public static void UpdateUser(int id, string name, string email, string plainTextPassword, UserType type) {
		updateUserAdmin(id, name, email, plainTextPassword, type);
	}

	public static void CreateUser(string name, string email, string plainTextPassword, UserType type) {
		createUserAdmin(name, email, plainTextPassword, type);
	}
}
