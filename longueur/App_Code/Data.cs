using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Common;

/// <summary>
/// Summary description for Data.
/// </summary>
public class Data {
	private static SqlConnection _sqlConnection;

	private Data()
	{
	}

	private static string encryptConnectionString()
	{
		Configuration configuration = WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
		ConfigurationSection section = configuration.GetSection(Constants.ConnectionStrings);

		if (section != null &&
			!section.SectionInformation.IsProtected)
		{
			section.SectionInformation.ProtectSection(Constants.DataProtectionConfigurationProvider);
			configuration.Save();
		}

		return ConfigurationManager.ConnectionStrings[Constants.ConnectionString].ConnectionString;
	}

	#region getSqlCommand
	private static SqlCommand getSqlCommand(string sql)
	{
		return getSqlCommand(sql, Constants.DefaultSqlCommandTimeout);
	}

	private static SqlCommand getSqlCommand(string sql, SqlParameter[] sqlParameters)
	{
		return getSqlCommand(sql, sqlParameters, Constants.DefaultSqlCommandTimeout);
	}

	private static SqlCommand getSqlCommand(string sql, int commandTimeout)
	{
		return getSqlCommand(sql, new SqlParameter[] { }, Constants.DefaultSqlCommandTimeout);
	}

	private static SqlCommand getSqlCommand(string sql, SqlParameter[] sqlParameters, int commandTimeout)
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
	private static SqlParameter[] getSqlParameters(string parameterList, params object[] values)
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

	public static DataTable GetQuoteDetails(int userId, int quoteId)
	{
		SqlParameter[] parameters = getSqlParameters("@UserID,@QuoteID", 
			userId, quoteId);

		//SqlParameter[] parameters = { new SqlParameter("@UserID", SqlDbType.Int),
		//    new SqlParameter("@QuoteID", SqlDbType.Int) };

		//parameters[0].Value = userId;
		//parameters[1].Value = quoteId;

		return Select("usp_QuoteGetDetails", parameters);
	}

	public static DataTable GetQuoteComments(int quoteId) {
		SqlParameter[] parameters = { new SqlParameter("@QuoteID", SqlDbType.Int) };
		parameters[0].Value = quoteId;

		return Select("usp_QuoteGetComments", parameters);
	}

	public static DataTable GetTopRated() {
		return Select("usp_AvgRatingTop");
	}

	public static DataTable GetNewest() {
		return Select("usp_QuoteGetNewest");
	}

	public static DataTable GetNews() {
		return Select("usp_NewsGet");
	}

	public static void CommentUpdate(int commentId, int userId, string commentText) {
		SqlParameter[] parameters = { new SqlParameter("@CommentID", SqlDbType.Int),
			new SqlParameter("@UserID", SqlDbType.Int),
			new SqlParameter("@CommentText", SqlDbType.VarChar, 8000) };

		parameters[0].Value = commentId;
		parameters[1].Value = userId;
		parameters[2].Value = commentText;

		NonQuery("usp_CommentUpdate", parameters);
	}

	public static void CommentInsert(int quoteId, int userId, string commentText) {
		SqlParameter[] parameters = { new SqlParameter("@QuoteID", SqlDbType.Int),
			new SqlParameter("@UserID", SqlDbType.Int),
			new SqlParameter("@CommentText", SqlDbType.VarChar, 8000) };

		parameters[0].Value = quoteId;
		parameters[1].Value = userId;
		parameters[2].Value = commentText;

		NonQuery("usp_CommentInsert", parameters);
	}

	public static void CommentDelete(int commentId, int userId) {
		SqlParameter[] parameters = { new SqlParameter("@CommentID", SqlDbType.Int),
			new SqlParameter("@UserID", SqlDbType.Int) };

		parameters[0].Value = commentId;
		parameters[1].Value = userId;

		NonQuery("usp_CommentDelete", parameters);
	}

	public static SiteUser GetAnonymousUser() {
		SiteUser user = new SiteUser();
		DataTable result = Select("usp_SiteUserGetAnonymous");

		if (result.Rows.Count == 1) {
			user = new SiteUser(int.Parse(result.Rows[0]["UserID"].ToString()), result.Rows[0]["UserName"].ToString(), "", result.Rows[0]["UserEmail"].ToString(), false, ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserRoleName"].ToString())));
			//user.Name = result.Rows[0]["UserName"].ToString();
			//user.Id = int.Parse(result.Rows[0]["UserID"].ToString());
			//user.Email = result.Rows[0]["UserEmail"].ToString();
			//user.Password = string.Empty;
			//user.UserRole = ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserRoleName"].ToString()));
			//user.EnableJs = false;
		}

		return user;
	}

	public static SiteUser GetUser(int userId) {
		SqlParameter[] parameters = { new SqlParameter("@UserID", SqlDbType.Int) };
		parameters[0].Value = userId;
		DataTable result = Select("usp_SiteUserGetUser", parameters);

		if (result.Rows.Count == 1) {
			SiteUser user = new SiteUser(int.Parse(result.Rows[0]["UserID"].ToString()), result.Rows[0]["UserName"].ToString(), result.Rows[0]["UserPassword"].ToString(), result.Rows[0]["UserEmail"].ToString(), false, ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserRoleName"].ToString())));

			//user.Name = result.Rows[0]["UserName"].ToString();
			//user.Id = int.Parse(result.Rows[0]["UserID"].ToString());
			//user.Email = result.Rows[0]["UserEmail"].ToString();
			//user.UserPassword = result.Rows[0]["UserPassword"].ToString();
			//user.UserRole = ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserRoleName"].ToString()));

			return user;
		} else {
			return null;
		}
	}

	public static SiteUser GetUser(string userName) {
		SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255) };
		parameters[0].Value = userName;
		DataTable result = Select("usp_SiteUserGetUser", parameters);

		if (result.Rows.Count == 1) {
			SiteUser user = new SiteUser(int.Parse(result.Rows[0]["UserID"].ToString()), result.Rows[0]["UserName"].ToString(), result.Rows[0]["UserPassword"].ToString(), result.Rows[0]["UserEmail"].ToString(), false, ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserRoleName"].ToString())));

			//user.UserName = result.Rows[0]["UserName"].ToString();
			//user.Id = int.Parse(result.Rows[0]["UserID"].ToString());
			//user.Email = result.Rows[0]["UserEmail"].ToString();
			//user.UserPassword = result.Rows[0]["UserPassword"].ToString();
			//user.UserRole = ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["UserRoleName"].ToString()));

			return user;
		} else {
			return null;
		}
	}

	public static bool UserExists(string userName) {
		if (GetUser(userName) == null) {
			return false;
		} else {
			return true;
		}
	}

	public static bool ValidateUser(string userName, string hashedPassword) {
		SiteUser user = GetUser(userName);

		if (user != null) {
			if (user.Password.Equals(hashedPassword)) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public static SiteUser UpdateUser(int userId, string userName, string userEmail) {
		return UpdateUser(userId, userName, userEmail, "");
	}

	public static SiteUser UpdateUser(int userId, string userName, string userEmail, string userPassword) {
		if (userPassword == string.Empty) {
			SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255),
			new SqlParameter("@UserEmail", SqlDbType.VarChar, 255),
			new SqlParameter("@UserID", SqlDbType.VarChar, 255) };

			parameters[0].Value = userName;
			parameters[1].Value = userEmail;
			parameters[2].Value = userId;

			NonQuery("usp_SiteUserUpdate", parameters);
		} else {
			SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255),
			new SqlParameter("@UserEmail", SqlDbType.VarChar, 255),
			new SqlParameter("@UserID", SqlDbType.VarChar, 255),
			new SqlParameter("@UserPassword", SqlDbType.VarChar, 255) };

			parameters[0].Value = userName;
			parameters[1].Value = userEmail;
			parameters[2].Value = userId;
			parameters[3].Value = Security.Encrypt(userPassword);

			NonQuery("usp_SiteUserUpdate", parameters);
		}

		return GetUser(userId);
	}

	public static void DeleteUser(int userId) {
		int anonId = GetAnonymousUser().Id;

		SqlParameter[] parameters = { new SqlParameter("@AnonUserId", SqlDbType.Int),
				new SqlParameter("@UserId", SqlDbType.Int) };

		parameters[0].Value = anonId;
		parameters[1].Value = userId;

		NonQuery("usp_SiteUserDelete", parameters);
	}

	public static SiteUser CreateUser(string userName, string userEmail, string userPassword) {
		DataTable result = new DataTable();

		SqlParameter[] parameters = { new SqlParameter("@UserName", SqlDbType.VarChar, 255),
			new SqlParameter("@UserEmail", SqlDbType.VarChar, 255),
			new SqlParameter("@UserPassword", SqlDbType.VarChar, 255) };

		parameters[0].Value = userName;
		parameters[1].Value = userEmail;
		parameters[2].Value = Security.Encrypt(userPassword);

		result = Select("usp_SiteUserInsert", parameters);

		if (result.Rows.Count > 0 && General.IsInt(result.Rows[0][0].ToString())) {
			return GetUser(Convert.ToInt32(result.Rows[0][0].ToString()));
		} else {
			return null;
		}
	}

	public static void RatingUpsert(int quoteId, int userId, int rating) {
		SqlParameter[] parameters = { new SqlParameter("@QuoteID", SqlDbType.Int),
			new SqlParameter("@UserID", SqlDbType.Int),
			new SqlParameter("@Rating", SqlDbType.Int) };

		parameters[0].Value = quoteId;
		parameters[1].Value = userId;
		parameters[2].Value = rating;

		NonQuery("usp_RatingUpsert", parameters);
	}

	public static void RatingDelete(int quoteId, int userId) {
		SqlParameter[] parameters = { new SqlParameter("@QuoteID", SqlDbType.Int),
			new SqlParameter("@UserID", SqlDbType.Int) };

		parameters[0].Value = quoteId;
		parameters[1].Value = userId;

		NonQuery("usp_RatingDelete", parameters);
	}

	public static int QuoteInsert(string artistName, string lyricistName, string songName, string albumName, string genreName, string quoteText, int userId) {
		SqlParameter[] parameters = { new SqlParameter("@ArtistName", SqlDbType.VarChar, 255),
			new SqlParameter("@LyricistName", SqlDbType.VarChar, 255),
			new SqlParameter("@SongName", SqlDbType.VarChar, 255),
			new SqlParameter("@AlbumName", SqlDbType.VarChar, 255),
			new SqlParameter("@GenreName", SqlDbType.VarChar, 255),
			new SqlParameter("@QuoteText", SqlDbType.VarChar, 255),
			new SqlParameter("@UserID", SqlDbType.Int) };

		parameters[0].Value = artistName == string.Empty ? Convert.DBNull : artistName;
		parameters[1].Value = lyricistName == string.Empty ? Convert.DBNull : lyricistName;
		parameters[2].Value = songName == string.Empty ? Convert.DBNull : songName;
		parameters[3].Value = albumName == string.Empty ? Convert.DBNull : albumName;
		parameters[4].Value = genreName == string.Empty ? Convert.DBNull : genreName;
		parameters[5].Value = quoteText;
		parameters[6].Value = userId;

		DataTable result = new DataTable();
		result = Select("usp_QuoteInsert", parameters);

		if (result.Rows.Count > 0) {
			return Convert.ToInt32(result.Rows[0][0]);
		} else {
			return -1;
		}
	}

	public static DataTable GetGenreList() {
		return Select("usp_GenreGetList");
	}

	public static DataSet AlbumGetDetails(string artistName, string albumName) {
		SqlParameter[] parameters = { new SqlParameter("@ArtistName", SqlDbType.VarChar, 255),
			new SqlParameter("@AlbumName", SqlDbType.VarChar, 255) };

		parameters[0].Value = artistName == string.Empty ? Convert.DBNull : artistName;
		parameters[1].Value = albumName == string.Empty ? Convert.DBNull : albumName;

		return SelectMultiple("usp_AlbumGetDetails", parameters);
	}

	public static void AlbumDetailsInsert(string artistName, string albumName) {
		//SqlParameter[] parameters = { new SqlParameter("@ArtistName", SqlDbType.VarChar, 255),
		//    new SqlParameter("@LyricistName", SqlDbType.VarChar, 255),
		//    new SqlParameter("@SongName", SqlDbType.VarChar, 255),
		//    new SqlParameter("@AlbumName", SqlDbType.VarChar, 255),
		//    new SqlParameter("@GenreName", SqlDbType.VarChar, 255),
		//    new SqlParameter("@QuoteText", SqlDbType.VarChar, 255),
		//    new SqlParameter("@UserID", SqlDbType.Int) };

		//parameters[0].Value = artistName == string.Empty ? Convert.DBNull : artistName;
		//parameters[1].Value = lyricistName == string.Empty ? Convert.DBNull : lyricistName;
		//parameters[2].Value = songName == string.Empty ? Convert.DBNull : songName;
		//parameters[3].Value = albumName == string.Empty ? Convert.DBNull : albumName;
		//parameters[4].Value = genreName == string.Empty ? Convert.DBNull : genreName;
		//parameters[5].Value = quoteText;
		//parameters[6].Value = userId;

		//DataTable result = new DataTable();
		//result = Select("usp_QuoteInsert", parameters);

		//if (result.Rows.Count > 0) {
		//    return Convert.ToInt32(result.Rows[0][0]);
		//} else {
		//    return -1;
		//}
	}

	public static DataTable SearchData(string searchString, SearchType searchType)
	{
		return SearchData(searchString, searchType, 10);
	}

	// searchtype should be an enum
	public static DataTable SearchData(string searchString, SearchType searchType, int rowCount)
	{
		SqlParameter[] parameters = { new SqlParameter("@SearchString", SqlDbType.Char, 1),
		    new SqlParameter("@SearchType", SqlDbType.VarChar, 100),
			new SqlParameter("@RowCount", SqlDbType.Int) };

		parameters[0].Value = searchString;
		parameters[1].Value = searchType.ToString();
		parameters[2].Value = rowCount;

		DataTable result = Select("usp_SearchData", parameters);

		return result;
	}

	public static void DownloadInsert(string filename, string IP, string referrer, string userAgent, 
		string browser, string platform, string browserVersion, string hostname)
	{
		SqlParameter[] parameters = getSqlParameters("@Filename,@IP,@Referrer,@UserAgent,@Browser,@Platform,@BrowserVersion,@HostName",
			filename, IP, referrer, userAgent, browser, platform, browserVersion, hostname);

		NonQuery("usp_DownloadInsert", parameters);
	}
}
