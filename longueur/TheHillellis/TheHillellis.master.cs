using System;
using System.Web.UI;
using System.Data;
using Data.Blog;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

public partial class TheHillellis_Default : MasterPage {
	protected void Page_Load(object sender, EventArgs e) {
		if (!IsPostBack) {
			// This should use the enum defined.
			Dictionary<string, int> themes = new Dictionary<string, int>(2);
			themes.Add("Spring", 1);
			themes.Add("Minimal", 2);

			ddlThemes.DataSource = themes;
			ddlThemes.DataTextField = "Key";
			ddlThemes.DataValueField = "Value";
			ddlThemes.DataBind();

			if (this.Request.Cookies["TheHillellis"] != null &&
				!string.IsNullOrEmpty(this.Request.Cookies["TheHillellis"]["Theme"])) {
				ThemeType theme = (ThemeType)Enum.Parse(typeof(ThemeType), this.Request.Cookies["TheHillellis"]["Theme"]);
				ddlThemes.SelectedValue = ((int)theme).ToString();
				styles.InnerHtml += "@import \"css/themes/" + theme.ToString() + ".css\";";
			} else {
				styles.InnerHtml += "@import \"css/themes/Spring.css\";";
			}

			Data.Blog.TheHillellis t = new TheHillellis();
			rptLinks.DataSource = t.GetLinks();
			rptLinks.DataBind();

			rptTags.DataSource = t.GetTags();
			rptTags.DataBind();

			if (rptTags.Items.Count == 0) {
				rptTags.Controls.Clear();
				rptTags.Controls.Add(new LiteralControl("Tags: None for now."));
			}

			rptRecent.DataSource = t.GetEntriesLimited(10);
			rptRecent.DataBind();

			if (rptRecent.Items.Count == 0) {
				rptRecent.Controls.Clear();
				rptRecent.Controls.Add(new LiteralControl("Recent entries: None for now."));
			}
		} else {
			HttpCookie cookie = new HttpCookie("TheHillellis");
			cookie.Values["Theme"] = ddlThemes.SelectedValue;
			cookie.Expires = DateTime.Now.AddYears(30);
			this.Response.Cookies.Add(cookie);

			ThemeType theme = (ThemeType)Enum.Parse(typeof(ThemeType), ddlThemes.SelectedValue);
			styles.InnerHtml += "@import \"css/themes/" + theme.ToString() + ".css\";";
		}
	}

	protected string getRecentDelimiter(RepeaterItem item) {
		string delimiter = string.Empty;

		if (item.ItemIndex < ((DataTable)rptRecent.DataSource).Rows.Count - 1) {
			delimiter = ",";
		}

		return delimiter;
	}

	protected string getTagDelimiter(RepeaterItem item) {
		string delimiter = string.Empty;

		if (item.ItemIndex < ((List<Tag>)rptTags.DataSource).Count - 1) {
			delimiter = ",";
		}

		return delimiter;
	}

	protected override void OnPreRender(EventArgs e) {
		base.OnPreRender(e);
		int archiveCount = 0;

		if (ctlLeftArchives != null && ctlRightArchives != null) {
			int leftCount = ctlLeftArchives.Count;
			int rightCount = ctlRightArchives.Count;

			if (leftCount == rightCount) {
				archiveCount = leftCount;
			} else if (leftCount < rightCount) {
				archiveCount = rightCount;
			} else {
				archiveCount = leftCount;
			}

			Session.Add("ArchiveCount", archiveCount);
		} else if (Session["ArchiveCount"] != null) {
			archiveCount = (int)Session["ArchiveCount"];
		}

        // TODO: Get a count of links to resize that correctly in the event that the window size 
        // is 800x600 and links are in their own box below the archives.
		string onLoadJavascript = @"
addListener(this, 'load', function() { numberOfArchives = " + archiveCount + @"; onLoad(); });
addListener(this, 'resize', function() { onLoad(); });
";

		this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "onLoad", onLoadJavascript, true);
	}
}
