using System;
using System.Data;
using System.Data.SqlClient;
using Data;
using Membership;
using Common;

namespace Data {
	/// <summary>
	/// Summary description for User
	/// </summary>
	public class User : Base {
		private User() {
		}

		public static DataTable GetAnonymousUser() {
			return Select("usp_SiteUserGetAnonymous");
		}

		public static DataTable GetUsers() {
			return Select("usp_SiteUserGet");
		}

		public static bool UserExists(string name) {
			if (GetUserData(name).Rows.Count > 0) {
				return true;
			}

			return false;
		}

		public static DataTable GetUserRoles() {
			return Select("usp_UserRoleGet");
		}

		public static void DeleteUser(int id) {
			int anonymousId = -1;
			DataTable result = GetAnonymousUser();

			if (result.Rows.Count == 1) {
				anonymousId = int.Parse(result.Rows[0]["Id"].ToString());
			}

			SqlParameter[] parameters = getSqlParameters("@AnonUserId,@Id",
				anonymousId,
				id);

			NonQuery("usp_SiteUserDelete", parameters);
		}

		public static int CreateUser(string name, string email, string password, UserType type) {
			DataTable result = new DataTable();

			SqlParameter[] parameters = getSqlParameters("@Name,@Email,@Password,@Type",
				name,
				email,
				Security.Encrypt(password),
				(int)type);

			result = Select("usp_SiteUserInsert", parameters);
			int id = 0;

			if (result.Rows.Count > 0) {
				int.TryParse(result.Rows[0][0].ToString(), out id);
			}

			return id;
		}

		public static void UpdateUser(int id, string name, string email, string plainTextPassword, UserType userType) {
			SqlParameter[] parameters;

			if (string.IsNullOrEmpty(plainTextPassword)) {
				parameters = getSqlParameters("@Name,@Email,@Id,@Type",
					name,
					email,
					id,
					userType);
			} else {
				parameters = getSqlParameters("@Name,@Email,@Id,@Password,@Type",
					name,
					email,
					id,
					Security.Encrypt(plainTextPassword),
					userType);
			}

			NonQuery("usp_SiteUserUpdate", parameters);
		}

		public static DataTable GetUserData(int id) {
			SqlParameter[] parameters = getSqlParameters("@Id",
				id);

			return getUserData(parameters);
		}

		public static DataTable GetUserData(string name) {
			SqlParameter[] parameters = getSqlParameters("@Name",
				name);

			return getUserData(parameters);
		}

		private static DataTable getUserData(SqlParameter[] parameters) {
			return Select("usp_SiteUserGetUser", parameters);
		}
	}
}