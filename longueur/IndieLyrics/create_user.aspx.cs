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
using System.Text.RegularExpressions;
using Common;
using Membership;

public partial class create_user : System.Web.UI.Page {
	protected void Page_Load(object sender, System.EventArgs e) {
		if (!IsPostBack) {
			if (Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("login.aspx")) {
				Session["ReferralUrl"] = Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "default.aspx";
			}

			UserName.Focus();
		}

		if (Session["CreateError"] != null) {
			CreateError.Text = Session["CreateError"].ToString();
			CreateError.Visible = true;
			Session["CreateError"] = null;
		} else {
			CreateError.Visible = false;
		}

		if (Session["ErrorUser"] != null && Session["ErrorEmail"] != null) {
			UserName.Text = Session["ErrorUser"].ToString();
			Email.Text = Session["ErrorEmail"].ToString();

			Session["ErrorUser"] = null;
			Session["ErrorEmail"] = null;
		}

		if (IsPostBack && Request.Form["MultiTaskType"] != null && Request.Form["MultiTaskType"] == "submit") {
			if (string.IsNullOrEmpty(UserName.Text)) {
				Session["CreateError"] += "Please enter a username<br />";
			} else if (!Regex.IsMatch(UserName.Text, @"[a-z|A-Z|0-9]")) {
				Session["CreateError"] += "Please enter a username consisting only of alpha-numerics.<br />";
			}

			if (string.IsNullOrEmpty(Pass.Text)) {
				Session["CreateError"] += "Please enter a password<br />";
			} else if (!Regex.IsMatch(Pass.Text, @"[a-z|A-Z|0-9]")) {
				Session["CreateError"] += "Please enter a password consisting only of alpha-numerics.<br />";
			}

			//if (SiteUser.Exists(UserName.Text))
			//{
			//    Session["CreateError"] += "The user, " + UserName.Text + " already exists, please try another.<br />";
			//}

			//SiteUser newUser = IndieLyrics.CreateUser(UserName.Text, Email.Text, Pass.Text);
			SiteUser newUser = new SiteUser();

			try {
				newUser = new SiteUser(UserName.Text, Email.Text, Pass.Text);
			} catch (ArgumentException ex) {
				Session["CreateError"] += "The user, " + UserName.Text + " already exists, please try another.<br />";
			}

			if (newUser == null) {
				Session["CreateError"] += "There was an error creating your user.<br />";
			}

			if (Session["CreateError"] == null) {
				Session[Constants.CurrentUser] = newUser;

				Response.Clear();
				Response.Redirect(Session["ReferralUrl"] != null ? Session["ReferralUrl"].ToString() : "default.aspx", true);
			} else {
				Session["ErrorUser"] = UserName.Text;
				Session["ErrorEmail"] = Email.Text;

				Response.Clear();
				Response.Redirect("create_user.aspx", true);
			}
		}
	}

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e) {
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
	}

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {
	}
	#endregion
}
