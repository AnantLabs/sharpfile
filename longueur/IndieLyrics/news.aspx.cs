using System;
using Data;

public partial class news : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		NewsItems.DataSource = IndieLyrics.GetNews();
		NewsItems.DataBind();
	}
}
