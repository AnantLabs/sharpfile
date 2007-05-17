using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Common;

namespace Data {
	/// <summary>
	/// Summary description for DownloadData
	/// </summary>
	public class Download : Base {
		private Download() {
		}

		public static void DownloadInsert(string filename, string IP, string referrer, string userAgent,
			string browser, string platform, string browserVersion, string hostname) {
			SqlParameter[] parameters = getSqlParameters("@Filename,@IP,@Referrer,@UserAgent,@Browser,@Platform,@BrowserVersion,@HostName",
				filename, IP, referrer, userAgent, browser, platform, browserVersion, hostname);

			NonQuery("usp_DownloadInsert", parameters);
		}
	}
}