<%@ WebHandler Language="C#" Class="Feed" %>
using System;
using System.Web;
using System.Xml;
using Data.Blog;
using Domain.Blog;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Configuration;
using System.Data.SqlClient;

public class Feed : IHttpHandler {
	private const string entriesXmlCacheKey = "entriesXml";
	private const int numberOfEntries = 10;

	public void ProcessRequest(HttpContext context) {
		string xmlString = getEntriesXmlFromCache();		
	    context.Response.ContentType = "text/xml";
	    context.Response.ContentEncoding = System.Text.Encoding.UTF8;
		context.Response.Write(xmlString); 
	}

	private string getEntriesXmlFromCache() {
		if (HttpContext.Current != null) {
			if (HttpContext.Current.Cache[entriesXmlCacheKey] == null) {

				string xml = buildXmlString();
				string connectionString = ConfigurationManager.ConnectionStrings[Constants.LongueurConnectionString].ConnectionString;

				HttpContext.Current.Cache.Add(entriesXmlCacheKey,
					xml,
					new SqlCacheDependency(new SqlCommand("SELECT * FROM TheHillellis", new SqlConnection(connectionString))),
					Cache.NoAbsoluteExpiration,
					new TimeSpan(0, 1, 0, 0),
					CacheItemPriority.Normal,
					null);
			}

			return (string)HttpContext.Current.Cache[entriesXmlCacheKey];
		} else {
			return buildXmlString();
		}
	}

	private string buildXmlString() {
		StringBuilder sb = new StringBuilder();

		using (XmlWriter xml = XmlWriter.Create(sb)) {
			xml.WriteStartDocument();
			xml.WriteStartElement("rss");
			xml.WriteAttributeString("version", "2.0");
			xml.WriteStartElement("channel");
			xml.WriteElementString("title", "The Hillellis");
			xml.WriteElementString("link", "http://www.longueur.org/thehillellis/feed.ashx");
			xml.WriteElementString("description", "Latest entries from the Hillellis!");
			xml.WriteElementString("ttl", "5");

			List<Entry> entries = EntriesFactory.GetEntriesLimited(new TheHillellis(), numberOfEntries);

			foreach (Entry entry in entries) {
				xml.WriteStartElement("item");
				xml.WriteElementString("title", HttpUtility.HtmlEncode(entry.Title));
				xml.WriteElementString("description", HttpUtility.HtmlEncode(entry.Content));
				xml.WriteElementString("link", "http://www.longueur.org/TheHillellis/Permalink.aspx?id=" + entry.Id.ToString());
				xml.WriteElementString("pubDate", entry.DateTime.ToString("R"));
				xml.WriteEndElement();
			}

			xml.WriteEndElement();
			xml.WriteEndElement();
			xml.WriteEndDocument();
			xml.Flush();
		}

		return sb.ToString();
	}
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}