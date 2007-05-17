using System;
using System.Web;
using System.IO;
using Data;

public partial class Software_download : System.Web.UI.Page
{
	private const string fileQueryString = "file";

	protected string filename;

	protected void Page_Load(object sender, EventArgs e)
	{
		// http://www.codeproject.com/aspnet/SecureFileDownload.asp.
		if (!string.IsNullOrEmpty(this.Request.QueryString[fileQueryString]))
		{
			filename = this.Request.QueryString[fileQueryString];
			string serverFilename = "~/software/" + filename;

			if (File.Exists(MapPath(serverFilename)))
			{
				string referrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : string.Empty;
				string niceFilename = filename.Substring(filename.LastIndexOf("/")+1, filename.Length - filename.LastIndexOf("/")-1);

				Download.DownloadInsert(filename, Request.UserHostAddress, referrer, 
					Request.UserAgent, Request.Browser.Browser, Request.Browser.Platform, 
					Request.Browser.Version, Request.UserHostName);

				if (Common.General.GetExt(filename).ToLower().Equals(".zip"))
				{
					Response.AddHeader("Content-disposition", "attachment; filename=" + niceFilename);
					Response.ContentType = "application/x-zip-compressed";
				}

				Server.Transfer(serverFilename, false);
			}
		}
	}
}
