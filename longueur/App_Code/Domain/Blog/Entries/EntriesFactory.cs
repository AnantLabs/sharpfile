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
			return new LynnEntries(themeType, entryTable);
		} else if (username.Equals("adam")) {
			return new AdamEntries(themeType, entryTable);
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

		return new AdamEntries(themeType, entryTable);
	}

	public static BaseEntries GetArchiveEntries(ThemeType themeType, IBlog blogDAO, int archiveId) {
		DataTable entriesTable = blogDAO.GetArchiveEntries(archiveId);
		string username = entriesTable.Rows[0]["Name"].ToString();

		if (username.Equals("lynn")) {
			return new LynnEntries(themeType, entriesTable);
		} else if (username.Equals("adam")) {
			return new AdamEntries(themeType, entriesTable);
		}

		return null;
	}
}
