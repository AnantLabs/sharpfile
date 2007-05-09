using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for AdminData
/// </summary>
public class AdminData : Data
{
	private AdminData()
	{
	}

	public static DataTable GetErrorLog()
	{
		return Select("usp_ErrorLogGet");
	}

	public static DataTable GetDownloads()
	{
		return Select("usp_DownloadGet");
	}
}
