using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

public partial class Software_download : System.Web.UI.Page
{
	private const string fileQueryString = "file";

	protected string filename;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(this.Request.QueryString[fileQueryString]))
		{
			filename = this.Request.QueryString[fileQueryString];
			string serverFilename = "~/software/" + filename;

			if (File.Exists(MapPath(serverFilename)))
			{
				Data.DownloadInsert(filename, Request.UserHostAddress, Request.UrlReferrer.ToString(), 
					Request.UserAgent, Request.Browser.Browser, Request.Browser.Platform, 
					Request.Browser.Version, Request.UserHostName);

				Server.Transfer(serverFilename);
			}
		}
	}
}
