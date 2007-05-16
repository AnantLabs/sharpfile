using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Common;
//using System.Text;

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

	protected static SqlParameter[] getSqlParameters(SqlParameter[] parameters, string parameterList, params object[] values) {
		SqlParameter[] newParameters = getSqlParameters(parameterList, values);

		int returnLength = parameters.Length + newParameters.Length;
		SqlParameter[] returnParameters = new SqlParameter[returnLength];

		for (int i = 0; i < parameters.Length; i++) {
			returnParameters[i] = parameters[i];
		}

		int returnParameterIndex = parameters.Length;

		for (int i = 0; i < newParameters.Length; i++) {
			returnParameters[returnParameterIndex] = newParameters[i];
			returnParameterIndex++;
		}

		return returnParameters;
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
	protected static DataTable Select(string sql)
	{
		return Select(sql, new SqlParameter[] { });
	}

	protected static DataTable Select(string sql, SqlParameter[] parameters)
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
	protected static DataSet SelectMultiple(string sql)
	{
		return SelectMultiple(sql, new SqlParameter[] { });
	}

	protected static DataSet SelectMultiple(string sql, SqlParameter[] parameters) {
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
	protected static void NonQuery(string sql) {
		NonQuery(sql, new SqlParameter[] { });
	}

	protected static void NonQuery(string sql, SqlParameter[] parameters) {
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
	protected static int Insert(string sql) {
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

	#region User methods
	public static DataTable GetAnonymousUser()
	{
		return Select("usp_SiteUserGetAnonymous");
	}

	public static DataTable GetUsers()
	{
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
