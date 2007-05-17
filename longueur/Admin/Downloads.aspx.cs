using System;
using Data;

public partial class Admin_Downloads : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		grdDownloads.DataSource = Admin.GetDownloads();
		grdDownloads.DataBind();
	}
}
