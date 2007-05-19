using System;
using System.Data;
using System.Data.SqlClient;
using Data;

namespace Data.Blog {
	/// <summary>
	/// Summary description for TheHillellies
	/// </summary>
	public class TheHillellies : Base, IBlog {
		public TheHillellies() {
		}

		#region IBlog Members
		public DataTable GetEntries() {
			return Select("usp_TheHillelliesGet");
		}

		public DataTable GetEntries(int userId) {
			SqlParameter[] parameters = getSqlParameters("@UserId",
				userId);

			return Select("usp_TheHillelliesGet", parameters);
		}

		public DataTable GetEntry(int id) {
			SqlParameter[] parameters = getSqlParameters("@Id",
				id);

			return Select("usp_TheHillelliesGetEntry", parameters);
		}

		public void InsertEntry(string title, string content, int userId) {
			SqlParameter[] parameters = getSqlParameters("@Content,@UserId,@Title",
				content, userId, title);

			NonQuery("usp_TheHillelliesInsert", parameters);
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

			NonQuery("usp_TheHillelliesUpdate", parameters);
		}
		#endregion
	}
}