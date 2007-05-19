using System.Data;

namespace Data.Blog {
	/// <summary>
	/// Summary description for IBlog
	/// </summary>
	public interface IBlog {
		DataTable GetEntries();
		DataTable GetEntries(int userId);
		DataTable GetEntry(int id);
		void InsertEntry(string title, string content, int userId);
		void DeleteEntry(int id);
		void UpdateEntry(int id, string title, string content, int userId);
	}
}