using System;
using System.Data;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Configuration;
using Data.Blog;

public partial class Controls_Blog_Archives : System.Web.UI.UserControl {
	private const string archivesCacheKey = "archives";

	private string name;
	private string title;

	protected void Page_Load(object sender, EventArgs e) {
		if (!IsPostBack) {
			//Archives archives = getArchivesFromCache();
			Archives archives = new Archives(new TheHillellis(), name);

			rptArchives.DataSource = archives;
			rptArchives.DataBind();

			if (rptArchives.Items.Count == 0) {
				rptArchives.Visible = false;
				lblNoArchives.Visible = true;
			}

			if (name.Equals("lynn")) {
				lblLeftTitle.Text = title;
				divLeftArchive.Visible = true;
				divRightArchive.Visible = false;
			} else if (name.Equals("adam")) {
				lblRightTitle.Text = title;
				divLeftArchive.Visible = false;
				divRightArchive.Visible = true;
			}
		}
	}

	//private Archives getArchivesFromCache() {
	//    if (Cache[archivesCacheKey + "_" + name] == null) {
	//        Archives archives = new Archives(new TheHillellis(), name);
	//        string connectionString = ConfigurationManager.ConnectionStrings[Constants.LongueurConnectionString].ConnectionString;

	//        Cache.Add(archivesCacheKey + "_" + name,
	//            archives,
	//            new SqlCacheDependency(new SqlCommand("SELECT * FROM Archive", new SqlConnection(connectionString))),
	//            Cache.NoAbsoluteExpiration,
	//            new TimeSpan(1, 0, 0, 0),
	//            CacheItemPriority.Normal,
	//            null);
	//    }

	//    return (Archives)Cache[archivesCacheKey + "_" + name];
	//}

	public int Count {
		get {
			return rptArchives.Items.Count == 0 ? 1 : rptArchives.Items.Count;
		}
	}

	public string Name {
		get {
			return name;
		}
		set {
			name = value;
		}
	}

	public string Title { 
		get { 
			return title; 
		} 
		set { 
			title = value; 
		} 
	}
}
