using System;
using System.Web.UI;
using System.Data;
using Data.Blog;

public partial class TheHillellis_Default : MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack) {
			rptArchives.DataSource = new Archives(new TheHillellis());
			rptArchives.DataBind();

			string onLoadJavascript = @"
addListener(this, 'load', function() { numberOfArchives = " + rptArchives.Items.Count + @"; onLoad(); });
addListener(this, 'resize', function() { onLoad(); });
";

			this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "onLoad", onLoadJavascript, true);
		}
	}
}
