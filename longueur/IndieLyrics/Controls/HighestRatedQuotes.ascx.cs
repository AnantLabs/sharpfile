using System;
using Data;

public partial class Controls_HighestRatedQuotes : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		HighestRepeater.DataSource = IndieLyrics.GetTopRated();
		HighestRepeater.DataBind();
	}
}
