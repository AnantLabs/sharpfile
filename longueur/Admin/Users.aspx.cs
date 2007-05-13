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

public partial class Admin_Users : System.Web.UI.Page
{
	private const string _id = "id";

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			int id = 0;

			if (string.IsNullOrEmpty(Request.QueryString[_id]))
			{
				rptUsers.DataSource = Data.GetUsers();
				rptUsers.DataBind();
			}
			else if (int.TryParse(Request.QueryString[_id], out id))
			{
				rptUsers.Visible = false;

				SiteUser siteUser = AdminData.GetUser(id);

				if (siteUser != null)
				{
					divUserInfo.Visible = true;

					txtName.Text = siteUser.Name;
					txtEmail.Text = siteUser.Email;
					lblDateTime.Text = siteUser.DateTime.ToString();
					lblId.Text = siteUser.Id.ToString();

					ddlUserType.DataSource = Data.GetUserRoles();
					ddlUserType.DataTextField = "TypeName";
					ddlUserType.DataValueField = "Type";
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
	}

	void btnSave_Click(object sender, EventArgs e) {
		int id = 0;

		if (int.TryParse(Request.QueryString[_id], out id)) {
			SiteUser siteUser = AdminData.GetUser(id);

			AdminData.UpdateUser(siteUser.Id, txtName.Text, txtEmail.Text, txtPassword.Text);
			Response.Redirect(Request.Url.PathAndQuery, true);
		} else {
			lblMessage.Visible = true;
			lblMessage.Text = "Oh no, looks like something went wacky-tacky.<br />That user doesn't seem to exist.";
		}
	}
}
