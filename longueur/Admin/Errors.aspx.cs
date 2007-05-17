using System;
using Data;

public partial class Admin_Errors : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		grdErrors.DataSource = Admin.GetErrorLog();
		grdErrors.DataBind();
	}
}
