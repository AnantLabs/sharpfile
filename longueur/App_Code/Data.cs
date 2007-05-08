using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Common;
using System.Text;

/// <summary>
/// Summary description for Data.
/// </summary>
public abstract class Data {
	private const string encryptConnectionStrings = "encryptConnectionStrings";
	private static SqlConnection _sqlConnection;

	private static string encryptConnectionString()
	{
		Configuration configuration = WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
		ConfigurationSection section = configuration.GetSection(Constants.ConnectionStrings);

		if (section != null &&
			!section.SectionInformation.IsProtected)
		{
			if (configuration.AppSettings.Settings[encryptConnectionStrings] != null)
			{
				bool outResult = false;

				if (bool.TryParse(configuration.AppSettings.Settings[encryptConnectionStrings].Value, out outResult))
				{
					if (outResult)
					{
						section.SectionInformation.ProtectSection(Constants.DataProtectionConfigurationProvider);
						configuration.Save(ConfigurationSaveMode.Modified);
					}
				}
			}
		}

		return ConfigurationManager.ConnectionStrings[Constants.LongueurConnectionString].ConnectionString;
	}

	#region getSqlCommand
	protected static SqlCommand getSqlCommand(string sql)
	{
		return getSqlCommand(sql, Constants.DefaultSqlCommandTimeout);
	}

	protected static SqlCommand getSqlCommand(string sql, SqlParameter[] sqlParameters)
	{
		return getSqlCommand(sql, sqlParameters, Constants.DefaultSqlCommandTimeout);
	}

	protected static SqlCommand getSqlCommand(string sql, int commandTimeout)
	{
		return getSqlCommand(sql, new SqlParameter[] { }, Constants.DefaultSqlCommandTimeout);
	}

	protected static SqlCommand getSqlCommand(string sql, SqlParameter[] sqlParameters, int commandTimeout)
	{
		using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
		{
			sqlCommand.CommandTimeout = commandTimeout;

			if (sql.StartsWith(Constants.StartOfStoredProcedure))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;

				if (sqlParameters.Length > 0)
				{
					sqlCommand.Parameters.AddRange(sqlParameters);
				}
			}
			else
			{
				sqlCommand.CommandType = CommandType.Text;
			}

			return sqlCommand;
		}
	}
	#endregion

	#region getSqlParameters
	protected static SqlParameter[] getSqlParameters(string parameterList, params object[] values)
	{
		string[] parameterArray = parameterList.Split(',');

		if (parameterArray.Length != values.Length)
		{
			throw new Exception("The number of parameters must equal the number of values passed in.");
		}

		SqlParameter[] parameters = new SqlParameter[parameterArray.Length];

		for (int i = 0; i < parameterArray.Length; i++)
		{
			string parameterName = parameterArray[i].Trim();
			object value = values[i];

			parameters[i] = new SqlParameter(parameterName, value);
		}

		return parameters;
	}
	#endregion

	#region Private getters
	private static SqlConnection sqlConnection
	{
		get
		{
			if (_sqlConnection == null)
			{
				string connectionString = encryptConnectionString();
				_sqlConnection = new SqlConnection(connectionString);
			}

			return _sqlConnection;
		}
	}
	#endregion

	#region Select
	public static DataTable Select(string sql)
	{
		return Select(sql, new SqlParameter[] { });
	}

	public static DataTable Select(string sql, SqlParameter[] parameters)
	{
		using (DataSet dataSet = SelectMultiple(sql, parameters))
		{
			if (dataSet != null &&
				dataSet.Tables.Count > 0)
			{
				return dataSet.Tables[0];
			}
		}

		return null;
	}
	#endregion

	#region SelectMultiple
	public static DataSet SelectMultiple(string sql)
	{
		return SelectMultiple(sql, new SqlParameter[] { });
	}

	public static DataSet SelectMultiple(string sql, SqlParameter[] parameters) {
		using (DataSet dataSet = new DataSet())
		{
			using (SqlCommand sqlCommand = getSqlCommand(sql, parameters))
			{
				using (SqlDataAdapter da = new SqlDataAdapter())
				{
					da.SelectCommand = sqlCommand;

					try
					{
						sqlConnection.Open();
						da.Fill(dataSet);
					}
					catch (Exception ex)
					{
						throw ex;
					}
					finally
					{
						sqlConnection.Close();
					}

					if (dataSet.Tables.Count > 0)
					{
						return dataSet;
					}
				}
			}
		}

		return null;
	}
	#endregion

	#region NonQuery
	public static void NonQuery(string sql) {
		NonQuery(sql, new SqlParameter[] { });
	}

	public static void NonQuery(string sql, SqlParameter[] parameters) {
		using (SqlCommand sqlCommand = getSqlCommand(sql, parameters))
		{
			try
			{
				sqlConnection.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				sqlConnection.Close();
			}
		}
	}
	#endregion

	#region Insert
	public static int Insert(string sql) {
		int ident = -1;

		if (sql.TrimEnd().EndsWith(";"))
		{
			sql = string.Format("{0} SELECT SCOPE_IDENTITY();", 
				sql.Trim());
		}
		else
		{
			sql = string.Format("{0}; SELECT SCOPE_IDENTITY();", 
				sql.Trim());
		}

		using (DataTable dataTable = Select(sql))
		{
			if (dataTable != null &&
				dataTable.Rows.Count > 0 &&
				dataTable.Rows[0][0] != null &&
				General.IsInt(dataTable.Rows[0][0].ToString()))
			{
				ident = int.Parse(dataTable.Rows[0][0].ToString());
			}
		}

		return ident;
	}
	#endregion

	#region Legacy user data calls
	public static SiteUser GetAnonymousUser()
	{
		SiteUser user = new SiteUser();
		DataTable result = Select("usp_SiteUserGetAnonymous");

		if (result.Rows.Count == 1)
		{
			user = new SiteUser(int.Parse(result.Rows[0]["UserID"].ToString()), result.Rows[0]["UserName"].ToString(), "", result.Rows[0]["UserEmail"].ToString(), false, ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserTypeName"].ToString())));
		}

		return user;
	}

	public static SiteUser GetUser(int userId, string plainTextPassword)
	{
		if (ValidateUser(userId, plainTextPassword))
		{
			return getUser(userId);
		}

		return null;
	}

	private static SiteUser getUser(int userId)
	{
		SqlParameter[] parameters = { new SqlParameter("@UserID", SqlDbType.Int) };
		parameters[0].Value = userId;
		DataTable result = Select("usp_SiteUserGetUser", parameters);

		if (result.Rows.Count == 1)
		{
			return new SiteUser(int.Parse(result.Rows[0]["UserID"].ToString()), result.Rows[0]["UserName"].ToString(), result.Rows[0]["UserPassword"].ToString(), result.Rows[0]["UserEmail"].ToString(), false, ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserTypeName"].ToString())));
		}

		return null;
	}

	public static SiteUser GetUser(string userName, string plainTextPassword)
	{
		if (ValidateUser(userName, plainTextPassword))
		{
			return getUser(userName);
		}

		return null;
	}

	private static SiteUser getUser(string userName)
	{
		SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255) };
		parameters[0].Value = userName;
		DataTable result = Select("usp_SiteUserGetUser", parameters);

		if (result.Rows.Count == 1)
		{
			return new SiteUser(int.Parse(result.Rows[0]["UserID"].ToString()), result.Rows[0]["UserName"].ToString(), result.Rows[0]["UserPassword"].ToString(), result.Rows[0]["UserEmail"].ToString(), false, ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserTypeName"].ToString())));
		}

		return null;
	}

	public static bool UserExists(string userName)
	{
		if (getUser(userName) == null)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	public static bool ValidateUser(string userName, string plainTextPassword)
	{
		string hashedPassword = Security.Encrypt(plainTextPassword);
		SiteUser user = getUser(userName);

		if (user != null)
		{
			if (user.HashedPassword.Equals(hashedPassword))
			{
				return true;
			}
		}

		return false;
	}

	public static bool ValidateUser(int userId, string plainTextPassword)
	{
		string hashedPassword = Security.Encrypt(plainTextPassword);
		SiteUser user = getUser(userId);

		if (user != null)
		{
			if (user.HashedPassword.Equals(hashedPassword))
			{
				return true;
			}
		}

		return false;
	}

	public static SiteUser UpdateUser(int userId, string userName, string userEmail)
	{
		return UpdateUser(userId, userName, userEmail, "");
	}

	public static SiteUser UpdateUser(int userId, string userName, string userEmail, string plainTextPassword)
	{
		if (!string.IsNullOrEmpty(plainTextPassword))
		{
			SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255),
			new SqlParameter("@UserEmail", SqlDbType.VarChar, 255),
			new SqlParameter("@UserID", SqlDbType.VarChar, 255) };

			parameters[0].Value = userName;
			parameters[1].Value = userEmail;
			parameters[2].Value = userId;

			NonQuery("usp_SiteUserUpdate", parameters);
		}
		else
		{
			SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255),
			new SqlParameter("@UserEmail", SqlDbType.VarChar, 255),
			new SqlParameter("@UserID", SqlDbType.VarChar, 255),
			new SqlParameter("@UserPassword", SqlDbType.VarChar, 255) };

			parameters[0].Value = userName;
			parameters[1].Value = userEmail;
			parameters[2].Value = userId;
			parameters[3].Value = Security.Encrypt(plainTextPassword);

			NonQuery("usp_SiteUserUpdate", parameters);
		}

		return getUser(userId);
	}

	public static void DeleteUser(int userId)
	{
		int anonId = GetAnonymousUser().Id;

		SqlParameter[] parameters = { new SqlParameter("@AnonUserId", SqlDbType.Int),
				new SqlParameter("@UserId", SqlDbType.Int) };

		parameters[0].Value = anonId;
		parameters[1].Value = userId;

		NonQuery("usp_SiteUserDelete", parameters);
	}

	public static SiteUser CreateUser(string userName, string userEmail, string userPassword)
	{
		DataTable result = new DataTable();

		SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255),
			new SqlParameter("@UserEmail", SqlDbType.VarChar, 255),
			new SqlParameter("@UserPassword", SqlDbType.VarChar, 255) };

		parameters[0].Value = userName;
		parameters[1].Value = userEmail;
		parameters[2].Value = Security.Encrypt(userPassword);

		result = Select("usp_SiteUserInsert", parameters);

		if (result.Rows.Count > 0 && General.IsInt(result.Rows[0][0].ToString()))
		{
			return getUser(Convert.ToInt32(result.Rows[0][0].ToString()));
		}
		else
		{
			return null;
		}
	}
	#endregion

	#region ErrorLog
	public static void InsertErrorLog(string message, string stacktrace, string ip, string url)
	{
		SqlParameter[] parameters = getSqlParameters("@Message,@Stacktrace,@IP,@Url",
			message, stacktrace, ip, url);

		NonQuery("usp_ErrorLogInsert", parameters);
	}
	#endregion
}
