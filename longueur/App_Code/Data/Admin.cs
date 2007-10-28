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

		public static void InsertErrorLog(string message, string details) {
			string ip = string.Empty;
			string url = string.Empty;

			if (HttpContext.Current != null) {
				// IP can also be got from HttpContext.Current.Request.UserHostAddress
				ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

				if (string.IsNullOrEmpty(ip)) {
					ip = HttpContext.Current.Request.UserHostAddress;
				}

				url = HttpContext.Current.Request.Url.PathAndQuery;
			}

			insertErrorLog(message, details, ip, url);
		}

		private static void insertErrorLog(Exception exception, string ip, string url) {
			insertErrorLog(exception.Message, exception.StackTrace, ip, url);
		}

		private static void insertErrorLog(string message, string stacktrace, string ip, string url) {
			SqlParameter[] parameters = getSqlParameters("@Message,@Stacktrace,@IP,@Url",
				message, stacktrace, ip, url);

			NonQuery("usp_ErrorLogInsert", parameters);
		}
	}
}