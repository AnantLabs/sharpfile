using System;
using System.Data;
using System.Collections.Generic;
using Data.Blog;
using Domain.Membership;

namespace Domain.Blog {
	/// <summary>
	/// Summary description for Entries
	/// </summary>
	public abstract class BaseEntries : List<Entry> {
		protected IBlog blogDAO;
		protected string backgroundColor;
		protected string titleImageUrl;

		public BaseEntries(DataTable entryTable)
			: base(entryTable.Rows.Count) {

			populateList(entryTable);
			setCustomAttributes();
		}

		public BaseEntries(IBlog blogDAO)
			: base() {
			this.blogDAO = blogDAO;
			DataTable entryTable = blogDAO.GetEntries();

			populateList(entryTable);
			setCustomAttributes();
		}

		public BaseEntries(IBlog blogDAO, int entryId)
			: base() {
			this.blogDAO = blogDAO;
			DataTable entryTable = blogDAO.GetEntry(entryId);

			populateList(entryTable);
			setCustomAttributes();
		}

		public BaseEntries(IBlog blogDAO, string userName)
			: base() {
			this.blogDAO = blogDAO;
			int userId = SiteUser.GetId(userName);
			DataTable entryTable = blogDAO.GetEntries(userId);

			populateList(entryTable);
			setCustomAttributes();
		}

		protected void populateList(DataTable entryTable) {
			foreach (DataRow row in entryTable.Rows) {
				Entry entry = new Entry(row);

				if (entry != null) {
					this.Add(entry);
				}
			}
		}

		protected abstract void setCustomAttributes();

		public string BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }
		public string TitleImageUrl { get { return titleImageUrl; } set { titleImageUrl = value; } }
	}
}