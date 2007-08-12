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
		public LynnEntries(ThemeType themeType, IBlog blogDAO, DataTable entryTable)
			: base(themeType, blogDAO, entryTable) {
		}

		public LynnEntries(ThemeType themeType, IBlog blogDAO)
			: base(themeType, blogDAO) {
		}

		public LynnEntries(ThemeType themeType, IBlog blogDAO, int entryId)
			: base(themeType, blogDAO, entryId) {
		}

		public LynnEntries(ThemeType themeType, IBlog blogDAO, string userName)
			: base(themeType, blogDAO, userName) {
		}

		protected override void setCustomAttributes() {
			titleImageUrl = "~/TheHillellis/Images/cupcake_t.png";

			switch (themeType) {
				case ThemeType.Spring:
					backgroundColor = "#CC99FF";
					break;
				case ThemeType.Minimal:
					backgroundColor = "#EEEEEE";
					break;
			}
		}
	}
}