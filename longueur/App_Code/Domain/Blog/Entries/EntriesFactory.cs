using System;
using Data.Blog;
using Domain.Blog;
using System.Data;

/// <summary>
/// Summary description for EntryFactory
/// </summary>
public static class EntriesFactory {
	public static BaseEntries GetEntries(IBlog blogDAO) {
		// Basically because the entry type doesn't really matter.
		return new AdamEntries(blogDAO);
	}

	public static BaseEntries GetEntries(IBlog blogDAO, int entryId) {
		DataTable entryTable = blogDAO.GetEntry(entryId);
		string username = entryTable.Rows[0]["Name"].ToString();

		if (username.Equals("lynn")) {
			return new LynnEntries(entryTable);
		} else if (username.Equals("adam")) {
			return new AdamEntries(entryTable);
		}

		return null;
	}

	public static BaseEntries GetEntries(IBlog blogDAO, string username) {
		if (username.Equals("lynn")) {
			return new LynnEntries(blogDAO, username);
		} else if (username.Equals("adam")) {
			return new AdamEntries(blogDAO, username);
		}

		return null;
	}

	public static BaseEntries GetArchiveEntries(IBlog blogDAO, int archiveId) {
		DataTable entriesTable = blogDAO.GetArchiveEntries(archiveId);
		string username = entriesTable.Rows[0]["Name"].ToString();

		if (username.Equals("lynn")) {
			return new LynnEntries(entriesTable);
		} else if (username.Equals("adam")) {
			return new AdamEntries(entriesTable);
		}

		return null;
	}
}
