using System;
using System.Collections.Generic;
using System.Data;
using Data.Blog;
using Domain.Membership;

namespace Domain.Blog {
	/// <summary>
	/// Summary description for Entries
	/// </summary>
	public class LynnEntries : BaseEntries {
		public LynnEntries(DataTable entryTable)
			: base(entryTable) {
		}

		public LynnEntries(IBlog blogDAO)
			: base(blogDAO) {
		}

		public LynnEntries(IBlog blogDAO, int entryId)
			: base(blogDAO, entryId) {
		}

		public LynnEntries(IBlog blogDAO, string userName)
			: base(blogDAO, userName) {
		}

		//public LynnEntries(DataTable entryTable)
		//    : base() {

		//    populateList(entryTable);
		//}

		//public LynnEntries(IBlog blogDAO)
		//    : base() {
		//    this.blogDAO = blogDAO;
		//    DataTable entryTable = blogDAO.GetEntries();

		//    populateList(entryTable);
		//}

		//public LynnEntries(IBlog blogDAO, int entryId)
		//    : base() {
		//    this.blogDAO = blogDAO;
		//    DataTable entryTable = blogDAO.GetEntry(entryId);

		//    populateList(entryTable);
		//}

		//public LynnEntries(IBlog blogDAO, string userName)
		//    : base() {
		//    this.blogDAO = blogDAO;
		//    int userId = SiteUser.GetId(userName);
		//    DataTable entryTable = blogDAO.GetEntries(userId);

		//    populateList(entryTable);
		//}

		protected override void setCustomAttributes() {
			titleImageUrl = "~/TheHillellis/Images/cupcake_t.png";
			backgroundColor = "#CC99FF";
		}
	}
}