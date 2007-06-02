using System;
using System.Collections.Generic;
using System.Data;
using Data.Blog;

/// <summary>
/// Summary description for Archives
/// </summary>
public class Archives : List<Archive> {
	private IBlog blogDAO;

	public Archives(IBlog blogDAO) {
		this.blogDAO = blogDAO;

		populate();
	}

	public Archives(IBlog blogDAO, string name) {
		this.blogDAO = blogDAO;

		populate(name);
	}

	private void populate() {
		populate(null);
	}

	private void populate(string name) {
		if (blogDAO != null) {
			DataTable archiveTable;

			if (!string.IsNullOrEmpty(name)) {
				archiveTable = blogDAO.GetArchives(name);
			} else {
				archiveTable = blogDAO.GetArchives();
			}

			foreach (DataRow row in archiveTable.Rows) {
				int id = int.Parse(row["Id"].ToString());
				DateTime startDate = DateTime.Parse(row["StartDate"].ToString());
				DateTime endDate = DateTime.Parse(row["EndDate"].ToString());
				string userName = row["Name"].ToString();
				int count = int.Parse(row["Count"].ToString());

				Archive archive = new Archive(id, startDate, endDate, userName, count);
				this.Add(archive);
			}
		}
	}
}
