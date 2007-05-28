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

	private void populate() {
		if (blogDAO != null) {
			DataTable archiveTable = blogDAO.GetArchives();

			foreach (DataRow row in archiveTable.Rows) {
				int id = int.Parse(row["Id"].ToString());
				DateTime startDate = DateTime.Parse(row["StartDate"].ToString());
				DateTime endDate = DateTime.Parse(row["EndDate"].ToString());
				string name = row["Name"].ToString();

				Archive archive = new Archive(id, startDate, endDate, name);
				this.Add(archive);
			}
		}
	}
}
