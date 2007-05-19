using System.Data;
using System.Collections.Generic;

namespace Domain.Membership {
	/// <summary>
	/// Summary description for Users
	/// </summary>
	public class SiteUsers : List<SiteUser> {
		public SiteUsers() : base() {
			DataTable users = Data.Membership.GetUsers();

			if (users != null) {
				foreach (DataRow row in users.Rows) {
					SiteUser siteUser = new SiteUser(int.Parse(row["Id"].ToString()));
					this.Add(siteUser);
				}
			}
		}
	}
}
