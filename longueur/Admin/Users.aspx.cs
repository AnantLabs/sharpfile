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
using Membership;

public partial class Admin_Users : System.Web.UI.Page
{
	private const string _id = "id";
	private const string _typeName = "TypeName";
	private const string _type = "Type";
	private const string pageToRedirectTo = "Users.aspx";

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			int id = 0;

			if (string.IsNullOrEmpty(Request.QueryString[_id]))
			{
				//rptUsers.DataSource = Data.GetUsers();
				rptUsers.DataSource = new SiteUsers();
				rptUsers.DataBind();

				// TODO: This should use the enum already created, not the database.
				ddlNewUserType.DataSource = Data.Membership.GetUserRoles();
				ddlNewUserType.DataTextField = _typeName;
				ddlNewUserType.DataValueField = _type;
				ddlNewUserType.DataBind();
			}
			else if (int.TryParse(Request.QueryString[_id], out id))
			{
				divUserInfo.Visible = true;
				rptUsers.Visible = false;
				divNewUser.Visible = false;

				SiteUser siteUser = new SiteUser(id);

				if (siteUser != null)
				{
					txtName.Text = siteUser.Name;
					txtEmail.Text = siteUser.Email;
					lblDateTime.Text = siteUser.DateTime.ToString();
					lblId.Text = siteUser.Id.ToString();

					// TODO: This should use the enum already created, not the database.
					ddlUserType.DataSource = Data.Membership.GetUserRoles();
					ddlUserType.DataTextField = _typeName;
					ddlUserType.DataValueField = _type;
					ddlUserType.DataBind();

					ddlUserType.SelectedValue = ((int)siteUser.UserType).ToString();
				}
				else
				{
					lblMessage.Visible = true;
					lblMessage.Text = "Oh no, looks like something went wacky-tacky.<br />That user doesn't seem to exist.";
				}
			}
		}
	}

	protected override void OnInit(EventArgs e)
	{
		InitializeComponents();
		base.OnInit(e);
	}

	private void InitializeComponents()
	{
		btnSave.Click += new EventHandler(btnSave_Click);
		btnNewSave.Click += new EventHandler(btnNewSave_Click);
	}

	private bool authenticate() {
		bool authenticated = false;

		if (User.Identity.IsAuthenticated) {
			authenticated = true;
		} else {
			lblMessage.Text = "Looks like you aren't an admin user after all, jerk.";
			lblMessage.Visible = true;
		}

		return authenticated;
	}

	void btnNewSave_Click(object sender, EventArgs e) {
		if (authenticate()) {
			string name = txtNewName.Text;

			try {
				SiteUser siteUser = new SiteUser(txtNewName.Text, txtNewEmail.Text, txtNewPassword.Text, (UserType)Enum.Parse(typeof(UserType), ddlNewUserType.SelectedValue));
				Response.Redirect(pageToRedirectTo, true);
			} catch (ArgumentException ex) {
				lblMessage.Visible = true;
				lblMessage.Text = "Oh no, looks like something went wacky-tacky.<br />That user already exists.";
			}
		}
	}

	void btnSave_Click(object sender, EventArgs e) {
		if (authenticate()) {
			int id = 0;

			if (int.TryParse(Request.QueryString[_id], out id)) {
				SiteUser.Update(id, txtName.Text, txtEmail.Text, txtPassword.Text, (UserType)Enum.Parse(typeof(UserType), ddlUserType.SelectedValue));
				Response.Redirect(pageToRedirectTo, true);
			} else {
				lblMessage.Visible = true;
				lblMessage.Text = "Oh no, looks like something went wacky-tacky.<br />That user doesn't seem to exist.";
			}
		}
	}
}
