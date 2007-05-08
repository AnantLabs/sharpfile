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

public partial class Admin_Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		throw new Exception("fkljdas");
	}

	protected override void OnInit(EventArgs e)
	{
		InitializeComponent();
		base.OnInit(e);
	}

	private void InitializeComponent()
	{
		this.btnSubmit.Click += new EventHandler(btnSubmit_Click);
	}

	void btnSubmit_Click(object sender, EventArgs e)
	{
		SiteUser siteUser = Data.GetUser(txtUsername.Text, txtPassword.Text);

		if (siteUser != null &&
			siteUser.Login())
		{
			string redirectUrl = "/";

			if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
			{
				redirectUrl = Request.QueryString["ReturnUrl"];
			}

			Response.Redirect(redirectUrl);
		}
		else
		{
			Response.Write("No dice.");
		}
	}
}
