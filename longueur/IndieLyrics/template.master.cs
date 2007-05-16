using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Common;
using System.Xml;
using System.Web.Caching;
using System.Xml.Serialization;
using System.IO;

public partial class template : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, System.EventArgs e)
	{
		SiteUser siteUser = SiteUser.GetCurrentUser();

		if (siteUser.UserType == UserType.NonAuthenticated)
		{
			UserLiteral.Text = string.Format("<li><a href=\"login.aspx\"{0}>LOGIN</a>/<a href=\"create_user.aspx\"{1}>CREATE USER</a></li>",
				Request.Path.ToLower().EndsWith("login.aspx") ? " class=\"nav_on\"" : "",
				Request.Path.ToLower().EndsWith("create_user.aspx") ? " class=\"nav_on\"" : "");

			AddLink.Visible = false;
		}
		else if (siteUser.UserType == UserType.Admin)
		{
			UserLiteral.Text = string.Format("<li><a href=\"edit_user.aspx\"{0}>EDIT</a>/<a href=\"admin.aspx\"{1}>ADMIN</a>/<a href=\"logout.aspx\">LOGOUT</a> (" + siteUser.Name + ")</li>",
				Request.Path.ToLower().EndsWith("edit_user.aspx") ? " class=\"nav_on\"" : "",
				Request.Path.ToLower().EndsWith("admin.aspx") ? " class=\"nav_on\"" : "");

			AddLink.Visible = true;
		}
		else if (siteUser.UserType == UserType.User)
		{
			UserLiteral.Text = string.Format("<li><a href=\"edit_user.aspx\"{0}>EDIT</a>/<a href=\"logout.aspx\">LOGOUT</a> (" + siteUser.Name + ")</li>",
				Request.Path.ToLower().EndsWith("edit_user.aspx") ? " class=\"nav_on\"" : "");

			AddLink.Visible = true;
		}

		Page.ClientScript.RegisterClientScriptBlock(typeof(string), "getGuid", @"
function getGuid() {
	return 'fickle';
	/*return '" + siteUser.Id //siteUser.Guid 
				+ @"';*/
}
", true);

		if (Request.Path.ToLower().Contains("default.aspx"))
		{
			HomeLink.CssClass = "nav_on";
		}
		else if (Request.Path.ToLower().Contains("search.aspx"))
		{
			SearchLink.CssClass = "nav_on";
		}
		else if (Request.Path.ToLower().Contains("add_quote.aspx"))
		{
			AddLink.CssClass = "nav_on";
		}
		else if (Request.Path.ToLower().Contains("news.aspx"))
		{
			NewsLink.CssClass = "nav_on";
		}

		ltrWittism.Text = getWittism();
	}

	private string getWittism()
	{
		Wittisms wittisms;

		// Grab the the wittisms xml file from cache if we can, otherwise deserialize the object and add it to cache.
		if (Cache["wittisms"] == null)
		{
			CacheDependency cacheDependency = new CacheDependency(Server.MapPath("wittisms.xml"));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Wittisms));

			if (File.Exists(Server.MapPath("wittisms.xml")))
			{
				using (TextReader tr = new StreamReader(Server.MapPath("wittisms.xml")))
				{
					wittisms = (Wittisms)xmlSerializer.Deserialize(tr);
				}
			}
			else
			{
				wittisms = new Wittisms();
			}

			Cache.Add("wittisms", wittisms, cacheDependency, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
		}
		else
		{
			// If the object is in cache, then just grab it and go.
			wittisms = (Wittisms)Cache["wittisms"];
		}

		// Grab a random line number and show it for all to see.
		Random random = new Random();
		int numberOfLines = wittisms.Count;
		int line = random.Next(0, numberOfLines);

		return wittisms[line];
	}
}
