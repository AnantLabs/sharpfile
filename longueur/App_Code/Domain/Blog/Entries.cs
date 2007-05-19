using System;
using System.Collections.Generic;
using System.Data;
using Data.Blog;
using Domain.Membership;

namespace Domain.Blog {
	/// <summary>
	/// Summary description for Entries
	/// </summary>
	public class Entries : List<Entry> {
		private IBlog blogDAO;

		public Entries(IBlog blogDAO)
			: base() {
			this.blogDAO = blogDAO;
			DataTable entryTable = blogDAO.GetEntries();

			populateList(entryTable);
		}

		public Entries(IBlog blogDAO, string userName)
			: base() {
			this.blogDAO = blogDAO;
			int userId = SiteUser.GetId(userName);
			DataTable entryTable = blogDAO.GetEntries(userId);

			populateList(entryTable);
		}

		//public Entries(IBlog blogDAO, int userId)
		//    : base() {
		//    this.blogDAO = blogDAO;
		//    DataTable entryTable = blogDAO.GetEntries(userId);

		//    populateList(entryTable);
		//}

		private void populateList(DataTable entryTable) {
			foreach (DataRow row in entryTable.Rows) {
				Entry entry = new Entry(blogDAO, int.Parse(row["Id"].ToString()));
				this.Add(entry);
			}
		}
	}
}