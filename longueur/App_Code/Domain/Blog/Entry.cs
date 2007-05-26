using System;
using System.Data;
using Data.Blog;

namespace Domain.Blog {
	/// <summary>
	/// Summary description for Entry
	/// </summary>
	public class Entry {
		private int id;
		private string title;
		private string content;
		private int userId;
		private string name;
		private DateTime dateTime;

		private IBlog blogDAO;

		public Entry(IBlog blogDAO, int id) {
			this.blogDAO = blogDAO;
			DataTable entryTable = blogDAO.GetEntry(id);

			if (entryTable.Rows.Count > 0) {
				this.id = id;
				this.userId = int.Parse(entryTable.Rows[0]["UserId"].ToString());
				this.title = entryTable.Rows[0]["Title"].ToString();
				this.content = entryTable.Rows[0]["Content"].ToString();
				this.name = entryTable.Rows[0]["Name"].ToString();
				this.dateTime = DateTime.Parse(entryTable.Rows[0]["DateTime"].ToString());
			}
		}

		public Entry(DataRow entryRow) {
			if (entryRow != null) {
				this.id = int.Parse(entryRow["Id"].ToString());
				this.userId = int.Parse(entryRow["UserId"].ToString());
				this.title = entryRow["Title"].ToString();
				this.content = entryRow["Content"].ToString();
				this.name = entryRow["Name"].ToString();
				this.dateTime = DateTime.Parse(entryRow["DateTime"].ToString());
			}
		}

		public int Id { get { return id; } set { id = value; } }
		public string Title { get { return title; } set { title = value; } }
		public string Content { get { return content; } set { content = value; } }
		public int UserId { get { return userId; } set { userId = value; } }
		public string Name { get { return name; } set { name = value; } }
		public DateTime DateTime { get { return dateTime; } set { dateTime = value; } }
	}
}