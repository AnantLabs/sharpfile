using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Data;
using Data.DAL;

namespace Data.Blog {
	/// <summary>
	/// Summary description for TheHillellis
	/// </summary>
	public class TheHillellis : Base, IBlog {
		private const string timezoneDifferenceSetting = "timezoneDifference";

		public TheHillellis() {
		}

		#region IBlog Members
		public DataTable GetEntries() {
			return Select("usp_TheHillellisGet");
		}

		public DataTable GetEntries(int userId) {
			SqlParameter[] parameters = getSqlParameters("@UserId",
				userId);

			return Select("usp_TheHillellisGet", parameters);
		}

		public DataTable GetEntriesLimited(int rowcount) {
			SqlParameter[] parameters = getSqlParameters("@Rowcount",
				rowcount);

			return Select("usp_TheHillellisGet", parameters);
		}

		public DataTable GetEntry(int id) {
			SqlParameter[] parameters = getSqlParameters("@Id",
				id);

			return Select("usp_TheHillellisGetEntry", parameters);
		}

		public void InsertEntry(string title, string content, int userId, DateTime dateTime) {
			int timezoneDifference = 0;
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);

			if (configuration.AppSettings.Settings[timezoneDifferenceSetting] != null) {
                int.TryParse(configuration.AppSettings.Settings[timezoneDifferenceSetting].Value, out timezoneDifference);
			}

            DateTime adjustedDateTime = dateTime.AddHours(timezoneDifference);

            backupEntry(title, content, adjustedDateTime, userId, false);

			SqlParameter[] parameters = getSqlParameters("@Content,@UserId,@Title,@DateTime",
				content, 
				userId, 
				title, 
				adjustedDateTime);

			NonQuery("usp_TheHillellisInsert", parameters);
		}

		public void DeleteEntry(int id) {
			throw new Exception("The method or operation is not implemented.");
		}

		public void UpdateEntry(int id, string title, string content, int userId) {
            backupEntry(title, content, DateTime.Now, userId, true);

			SqlParameter[] parameters = getSqlParameters("@Id,@Content,@UserId,@Title",
				id, 
				content, 
				userId, 
				title);

			NonQuery("usp_TheHillellisUpdate", parameters);
		}

		public DataTable GetArchives() {
			return Select("usp_TheHillellisGetArchives");
		}

		public DataTable GetArchives(string name) {
			SqlParameter[] parameters = getSqlParameters("@Name",
				name);

			return Select("usp_TheHillellisGetArchives", parameters);
		}

		public DataTable GetArchiveEntries(int archiveId) {
			SqlParameter[] parameters = getSqlParameters("@ArchiveId", 
				archiveId);

			return Select("usp_TheHillellisGetArchiveEntries", parameters);
		}

		public List<Link> GetLinks() {
			return DBHelper.ReadCollection<Link>("usp_TheHillellisGetLinks");
		}

		public List<Tag> GetTags() {
			return DBHelper.ReadCollection<Tag>("usp_TheHillellisGetTags");
		}

		public List<Tag> GetEntryTags(int entryId) {
			SqlParameter[] sqlParameters = getSqlParameters("@EntryId", entryId);
			return DBHelper.ReadCollection<Tag>("usp_TheHillellisGetEntryTags", sqlParameters);
		}

		public DataSet GetTagEntries(int tagId) {
			SqlParameter[] parameters = getSqlParameters("@TagId",
				tagId);

			return SelectMultiple("usp_TheHillellisGetTagEntries", parameters);
		}
		#endregion

        private void backupEntry(string title, string content, DateTime dateTime, int userId, bool isUpdatedEntry)
        {
            try
            {
                string physicalPath = string.Format("{0}/Admin/TheHillellis/{1}_{2}",
                    HttpContext.Current.Request.MapPath(HttpContext.Current.Request.ApplicationPath),
                    dateTime.ToString("MMddyyyy_HHmmss"),
                    userId);

                if (isUpdatedEntry)
                {
                    physicalPath += "_u";
                }

                using (StreamWriter sw = File.CreateText(physicalPath + ".txt"))
                {
                    sw.WriteLine(title);
                    sw.WriteLine(content);
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                Data.Admin.InsertErrorLog(ex);
            }
        }
	}
}