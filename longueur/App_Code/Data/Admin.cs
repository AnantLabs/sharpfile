using System.Data;
using System.Data.SqlClient;
using System;
using System.Web;

namespace Data {
	/// <summary>
	/// Summary description for AdminData
	/// </summary>
	public class Admin : Base {
		private Admin() {
		}

		public static DataTable GetErrorLog() {
			return Select("usp_ErrorLogGet");
		}

		public static DataTable GetDownloads() {
			return Select("usp_DownloadGet");
		}

		public static void InsertErrorLog(Exception exception) {
			InsertErrorLog(exception.Message, exception.StackTrace);
		}

		public static void InsertErrorLog(string message, string stackTrace) {
			string ip = string.Empty;
			string url = string.Empty;

			if (HttpContext.Current != null) {
				// IP can also be got from HttpContext.Current.Request.UserHostAddress
				ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
				url = HttpContext.Current.Request.Url.PathAndQuery;
			}

			InsertErrorLog(message, stackTrace, ip, url);
		}

		public static void InsertErrorLog(Exception exception, string ip, string url) {
			InsertErrorLog(exception.Message, exception.StackTrace, ip, url);
		}

		public static void InsertErrorLog(string message, string stacktrace, string ip, string url) {
			SqlParameter[] parameters = getSqlParameters("@Message,@Stacktrace,@IP,@Url",
				message, stacktrace, ip, url);

			NonQuery("usp_ErrorLogInsert", parameters);
		}
	}
}