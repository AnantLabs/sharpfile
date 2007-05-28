using System;
using System.Web.UI;
using System.Data;
using Data.Blog;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Configuration;

public partial class TheHillellis_Default : MasterPage
{
	private const string archivesCacheKey = "archives";

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack) {
			Archives archives = getArchivesFromCache();

			rptArchives.DataSource = archives;
			rptArchives.DataBind();

			if (rptArchives.Items.Count == 0) {
				rptArchives.Visible = false;
				lblNoArchives.Visible = true;
			}

			string onLoadJavascript = @"
addListener(this, 'load', function() { numberOfArchives = " + rptArchives.Items.Count + @"; onLoad(); });
addListener(this, 'resize', function() { onLoad(); });
";

			this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "onLoad", onLoadJavascript, true);
		}
	}

	private Archives getArchivesFromCache() {
		if (Cache[archivesCacheKey] == null) {
			Archives archives = new Archives(new TheHillellis());
			string connectionString = ConfigurationManager.ConnectionStrings[Constants.LongueurConnectionString].ConnectionString;

			Cache.Add(archivesCacheKey,
				archives,
				new SqlCacheDependency(new SqlCommand("SELECT * FROM Archive", new SqlConnection(connectionString))),
				Cache.NoAbsoluteExpiration,
				new TimeSpan(1, 0, 0, 0),
				CacheItemPriority.Normal,
				null);
		}

		return (Archives)Cache[archivesCacheKey];
	}
}
