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

public partial class Controls_Login : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// Make sure our enter key is happy.
			txtPassword.Attributes.Add("onKeyPress", "javascript:if (event.keyCode == 13) { __doPostBack('" + lnkSubmit.UniqueID + "',''); }");

			message.Visible = false;
			txtUsername.Focus();
		}
	}

	protected override void OnInit(EventArgs e)
	{
		InitializeComponent();
		base.OnInit(e);
	}

	private void InitializeComponent()
	{
		this.lnkSubmit.Click += new EventHandler(lnkSubmit_Click);
	}

	void lnkSubmit_Click(object sender, EventArgs e)
	{
		//SiteUser siteUser = Data.GetUser(txtUsername.Text, txtPassword.Text);

		SiteUser siteUser = new SiteUser(txtUsername.Text);

		if (siteUser != null &&
			siteUser.Login())
		{
			string redirectUrl = "/";

			if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
			{
				redirectUrl = Request.QueryString["ReturnUrl"];
			}

			Response.Redirect(redirectUrl, true);
		}
		else
		{
			message.Visible = true;
			message.Text = "The username or password entered is incorrect. Try again.";
		}
	}
}
