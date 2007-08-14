using System;
using Data.Blog;
using Domain.Blog;
using System.Data;

/// <summary>
/// Summary description for EntryFactory
/// </summary>
public static class EntriesFactory {
	public static BaseEntries GetEntries(ThemeType themeType, IBlog blogDAO) {
		// Basically because the entry type doesn't really matter.
		return new AdamEntries(themeType, blogDAO);
	}

	public static BaseEntries GetEntries(ThemeType themeType, IBlog blogDAO, int entryId) {
		DataTable entryTable = blogDAO.GetEntry(entryId);
		string username = entryTable.Rows[0]["Name"].ToString();

		if (username.Equals("lynn")) {
			return new LynnEntries(themeType, blogDAO, entryTable);
		} else if (username.Equals("adam")) {
			return new AdamEntries(themeType, blogDAO, entryTable);
		}

		return null;
	}

	public static BaseEntries GetEntries(ThemeType themeType, IBlog blogDAO, string username) {
		if (username.Equals("lynn")) {
			return new LynnEntries(themeType, blogDAO, username);
		} else if (username.Equals("adam")) {
			return new AdamEntries(themeType, blogDAO, username);
		}

		return null;
	}

	public static AdamEntries GetEntriesLimited(ThemeType themeType, IBlog blogDAO, int rowcount) {
		DataTable entryTable = blogDAO.GetEntriesLimited(rowcount);

		return new AdamEntries(themeType, blogDAO, entryTable);
	}

	public static BaseEntries GetArchiveEntries(ThemeType themeType, IBlog blogDAO, int archiveId) {
		DataTable entriesTable = blogDAO.GetArchiveEntries(archiveId);
		string username = entriesTable.Rows[0]["Name"].ToString();

		if (username.Equals("lynn")) {
			return new LynnEntries(themeType, blogDAO, entriesTable);
		} else if (username.Equals("adam")) {
			return new AdamEntries(themeType, blogDAO, entriesTable);
		}

		return null;
	}

	public static BaseEntries GetTagEntries(ThemeType themeType, IBlog blogDAO, int tagId, out Tag tag) {
		DataSet dataSet = blogDAO.GetTagEntries(tagId);
		DataTable entriesTable = dataSet.Tables[0];
		DataTable tagTable = dataSet.Tables[1];

		int id = int.Parse(tagTable.Rows[0]["Id"].ToString());
		string name = tagTable.Rows[0]["Name"].ToString();
		string image = tagTable.Rows[0]["Image"].ToString();
		tag = new Tag(id, name, image);

		string username = entriesTable.Rows[0]["Name"].ToString();

		if (username.Equals("lynn")) {
			return new LynnEntries(themeType, blogDAO, entriesTable);
		} else if (username.Equals("adam")) {
			return new AdamEntries(themeType, blogDAO, entriesTable);
		}

		return null;
	}
}
