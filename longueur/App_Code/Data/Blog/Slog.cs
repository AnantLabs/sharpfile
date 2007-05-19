using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Common;

namespace Data.Blog {
	/// <summary>
	/// Summary description for SlogData
	/// </summary>
	public class Slog : Base, IBlog {
		public Slog() {
		}

		#region IBlog Members
		public DataTable GetEntries() {
			return Select("usp_SlogGet");
		}

		public DataTable GetEntries(int userId) {
			throw new Exception("The method or operation is not implemented.");
		}

		public DataTable GetEntry(int id) {
			SqlParameter[] parameters = getSqlParameters("@Id",
				id);

			return Select("usp_SlogGetSlog", parameters);
		}

		public void InsertEntry(string title, string content, int userId) {
			SqlParameter[] parameters = getSqlParameters("@Content,@UserId,@Title",
				content, userId, title);

			NonQuery("usp_SlogInsert", parameters);
		}

		public void DeleteEntry(int id) {
			throw new Exception("The method or operation is not implemented.");
		}

		public void UpdateEntry(int id, string title, string content, int userId) {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
	}
}