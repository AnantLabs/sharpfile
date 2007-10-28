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
using Data;

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
		bool validUsername = false;
		bool validLogin = false;
		string redirectUrl = "/";
		string username = txtUsername.Text;
		string password = txtPassword.Text;

		try {
			SiteUser siteUser = new SiteUser(username);

			if (siteUser != null) {
				validUsername = true;
				validLogin = siteUser.Login(password);
			}
		} catch (Exception ex) {
			validUsername = false;
			validLogin = false;
			Admin.InsertErrorLog(ex);
		}

		if (validUsername && validLogin) {
			Admin.InsertErrorLog("Valid user and password login.", "Username: " + username);

			if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"])) {
				redirectUrl = Request.QueryString["ReturnUrl"];
			}

			Response.Redirect(redirectUrl, true);
		} else {
			if (!validUsername) {
				Admin.InsertErrorLog("Invalid username login.", "Username: " + username);
			} else if (!validLogin) {
				Admin.InsertErrorLog("Invalid password login.", "Username: " + username);
			}

			message.Visible = true;
			message.Text = "The username or password entered is incorrect. Try again.";
		}
	}
}
