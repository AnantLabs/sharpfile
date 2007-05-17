using System;
using Data;

public partial class Controls_NewestQuotes : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		NewestRepeater.DataSource = IndieLyrics.GetNewest();
		NewestRepeater.DataBind();
	}
}
