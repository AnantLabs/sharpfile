using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using Common;

namespace Data {
	/// <summary>
	/// Summary description for LongueurData
	/// </summary>
	public class IndieLyrics : Base {
		private IndieLyrics() {
		}

		public static DataTable GetQuoteDetails(int userId, int quoteId) {
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

		public static DataTable SearchData(string searchString, SearchType searchType) {
			return SearchData(searchString, searchType, 10);
		}

		// searchtype should be an enum
		public static DataTable SearchData(string searchString, SearchType searchType, int rowCount) {
			SqlParameter[] parameters = { new SqlParameter("@SearchString", SqlDbType.Char, 1),
		    new SqlParameter("@SearchType", SqlDbType.VarChar, 100),
			new SqlParameter("@RowCount", SqlDbType.Int) };

			parameters[0].Value = searchString;
			parameters[1].Value = searchType.ToString();
			parameters[2].Value = rowCount;

			DataTable result = Select("usp_SearchData", parameters);

			return result;
		}
	}
}