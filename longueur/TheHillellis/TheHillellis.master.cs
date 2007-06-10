using System;
using System.Web.UI;
using System.Data;
using Data.Blog;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Configuration;

public partial class TheHillellis_Default : MasterPage
{
	protected void Page_Load(object sender, EventArgs e) {
		if (false) {
			styles.InnerHtml += "@import \"css/themes/minimal.css\";";
		}
	}

	protected override void OnPreRender(EventArgs e) {
		base.OnPreRender(e);

		if (!IsPostBack) {
			string onLoadJavascript = @"
addListener(this, 'load', function() { numberOfArchives = " + ctlLeftArchives.Count + @"; onLoad(); });
addListener(this, 'resize', function() { onLoad(); });
";

			this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "onLoad", onLoadJavascript, true);
		}
	}
}
