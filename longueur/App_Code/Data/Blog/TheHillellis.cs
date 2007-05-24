using System;
using System.Data;
using System.Data.SqlClient;
using Data;
using System.Configuration;
using System.Web.Configuration;

namespace Data.Blog {
	/// <summary>
	/// Summary description for TheHillellis
	/// </summary>
	public class TheHillellis : Base, IBlog {
		private const string timezoneDifferenceSetting = "timezoneDifference";

		public TheHillellis() {
		}

		#region IBlog Members
		public DataTable GetEntries() {
			return Select("usp_TheHillellisGet");
		}

		public DataTable GetEntries(int userId) {
			SqlParameter[] parameters = getSqlParameters("@UserId",
				userId);

			return Select("usp_TheHillellisGet", parameters);
		}

		public DataTable GetEntry(int id) {
			SqlParameter[] parameters = getSqlParameters("@Id",
				id);

			return Select("usp_TheHillellisGetEntry", parameters);
		}

		public void InsertEntry(string title, string content, int userId, DateTime dateTime) {
			int timezoneDifference = 0;
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);

			if (configuration.AppSettings.Settings[timezoneDifferenceSetting] != null) {
				dateTime.AddHours(timezoneDifference);
			}

			SqlParameter[] parameters = getSqlParameters("@Content,@UserId,@Title,@DateTime",
				content, 
				userId, 
				title, 
				dateTime.AddHours(timezoneDifference));

			NonQuery("usp_TheHillellisInsert", parameters);
		}

		public void DeleteEntry(int id) {
			throw new Exception("The method or operation is not implemented.");
		}

		public void UpdateEntry(int id, string title, string content, int userId) {
			SqlParameter[] parameters = getSqlParameters("@Id,@Content,@UserId,@Title",
				id, 
				content, 
				userId, 
				title);

			NonQuery("usp_TheHillellisUpdate", parameters);
		}
		#endregion
	}
}