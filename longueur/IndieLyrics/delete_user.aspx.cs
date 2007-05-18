using System;
using System.Web;
using Common;
using Membership;

public partial class delete_user : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) {
			Data.Membership.DeleteUser(((SiteUser)Session[Constants.CurrentUser]).Id);
		}

		Response.Clear();
		Response.Redirect("default.aspx", true);
    }
}
