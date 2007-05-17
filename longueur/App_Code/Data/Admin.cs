using System.Data;
using System.Data.SqlClient;

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

		public static void InsertErrorLog(string message, string stacktrace, string ip, string url) {
			SqlParameter[] parameters = getSqlParameters("@Message,@Stacktrace,@IP,@Url",
				message, stacktrace, ip, url);

			NonQuery("usp_ErrorLogInsert", parameters);
		}
	}
}