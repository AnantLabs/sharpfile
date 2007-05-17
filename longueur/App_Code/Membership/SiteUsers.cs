using System.Data;
using System.Collections.Generic;
using Data;

namespace Membership {
	/// <summary>
	/// Summary description for Users
	/// </summary>
	public class SiteUsers : List<SiteUser> {
		public SiteUsers()
			: base() {
			DataTable users = User.GetUsers();

			foreach (DataRow row in users.Rows) {
				SiteUser siteUser = new SiteUser(int.Parse(row["Id"].ToString()));
				this.Add(siteUser);
			}
		}
	}
}
