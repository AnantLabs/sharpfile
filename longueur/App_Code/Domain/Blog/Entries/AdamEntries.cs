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
		public AdamEntries(ThemeType themeType, DataTable entryTable)
			: base(themeType, entryTable) {
		}

		public AdamEntries(ThemeType themeType, IBlog blogDAO)
			: base(themeType, blogDAO) {
		}

		public AdamEntries(ThemeType themeType, IBlog blogDAO, int entryId)
			: base(themeType, blogDAO, entryId) {
		}

		public AdamEntries(ThemeType themeType, IBlog blogDAO, string userName)
			: base(themeType, blogDAO, userName) {
		}

		protected override void setCustomAttributes() {
			titleImageUrl = "~/TheHillellis/Images/puppup_t.png";

			switch (themeType) {
				case ThemeType.Spring:
					backgroundColor = "#99CCFF";
					break;
				case ThemeType.Minimal:
					backgroundColor = "#EEEEEE";
					break;
			}
		}
	}
}