using System;

public partial class logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Session[Constants.CurrentUser] = Data.User.GetAnonymousUser();

		Response.Clear();
		Response.Redirect("default.aspx", true);
    }
}
