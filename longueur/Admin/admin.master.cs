using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Domain.Membership;

public partial class Admin_template : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected override void OnInit(EventArgs e) {
		InitializeComponents();
		base.OnInit(e);
	}

	private void InitializeComponents() {
		lnkLogout.Click += new EventHandler(lnkLogout_Click);
	}

	void lnkLogout_Click(object sender, EventArgs e) {
		SiteUser siteUser = SiteUser.GetCurrentUser();

		if (siteUser != null) {
			siteUser.Logout();
		}
	}
}
