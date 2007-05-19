using System;
using Data.Blog;
using Domain.Blog;

public partial class Slog_Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// TODO: Randomize this bitch.
		this.Title = "Slog: Est. 1842.";

		rptContent.DataSource = new Entries(new Slog());
		rptContent.DataBind();
	}
}
