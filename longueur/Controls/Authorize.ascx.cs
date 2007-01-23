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

public partial class Controls_Authorize : System.Web.UI.UserControl
{
	private UserType userType = UserType.NonAuthenticated;
	private string redirect;
	private string username;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (Session[Constants.CurrentUser] != null)
		{
			SiteUser user = (SiteUser)Session[Constants.CurrentUser];

			if ((!string.IsNullOrEmpty(username) && !user.Name.Equals(username)) 
				|| user.UserType < userType)
			{
				if (string.IsNullOrEmpty(redirect))
				{
					this.Parent.Parent.FindControl("Content").Visible = false;
					this.Parent.Parent.FindControl("NotAuthorized").Visible = true;
				}
				else
				{
					Response.Clear();
					Response.Redirect(redirect, true);
				}
			}
		}
	}

	public string Redirect
	{
		get
		{
			return redirect;
		}
		set
		{
			redirect = value;
		}
	}

	public UserType UserType
	{
		get
		{
			return userType;
		}
		set
		{
			userType = value;
		}
	}

	public string Username
	{
		get
		{
			return username;
		}
		set
		{
			username = value;
		}
	}
}