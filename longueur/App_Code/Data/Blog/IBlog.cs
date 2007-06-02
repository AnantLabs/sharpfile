using System.Data;
using System;

namespace Data.Blog {
	/// <summary>
	/// Summary description for IBlog
	/// </summary>
	public interface IBlog {
		DataTable GetEntries();
		DataTable GetEntries(int userId);
		DataTable GetEntry(int id);
		void InsertEntry(string title, string content, int userId, DateTime dateTime);
		void DeleteEntry(int id);
		void UpdateEntry(int id, string title, string content, int userId);
		DataTable GetArchives();
		DataTable GetArchives(string name);
		DataTable GetArchiveEntries(int archiveId);
	}
}