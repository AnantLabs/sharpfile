using System;
using System.Collections.Generic;
using System.Data;
using Data.Blog;
using Domain.Membership;

namespace Domain.Blog {
	/// <summary>
	/// Summary description for Entries
	/// </summary>
	public class AdamEntries : BaseEntries {
		public AdamEntries(DataTable entryTable)
			: base(entryTable) {
		}

		public AdamEntries(IBlog blogDAO)
			: base(blogDAO) {
		}

		public AdamEntries(IBlog blogDAO, int entryId)
			: base(blogDAO, entryId) {
		}

		public AdamEntries(IBlog blogDAO, string userName)
			: base(blogDAO, userName) {
		}

		//public AdamEntries(DataTable entryTable)
		//    : base() {

		//    populateList(entryTable);
		//}

		//public AdamEntries(IBlog blogDAO)
		//    : base() {
		//    this.blogDAO = blogDAO;
		//    DataTable entryTable = blogDAO.GetEntries();

		//    populateList(entryTable);
		//}

		//public AdamEntries(IBlog blogDAO, int entryId)
		//    : base() {
		//    this.blogDAO = blogDAO;
		//    DataTable entryTable = blogDAO.GetEntry(entryId);

		//    populateList(entryTable);
		//}

		//public AdamEntries(IBlog blogDAO, string userName)
		//    : base() {
		//    this.blogDAO = blogDAO;
		//    int userId = SiteUser.GetId(userName);
		//    DataTable entryTable = blogDAO.GetEntries(userId);

		//    populateList(entryTable);
		//}

		protected override void setCustomAttributes() {
			titleImageUrl = "~/TheHillellis/Images/puppup_t.png";
			backgroundColor = "#99CCFF";
		}
	}
}