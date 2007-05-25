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
using System.Web.Configuration;

public partial class TheHillellis_tester : System.Web.UI.Page {
	protected void Page_Load(object sender, EventArgs e) {
		int timezoneDifference = 0;
		Configuration configuration = WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);

		if (configuration.AppSettings.Settings["timezoneDifference"] != null) {
			if (int.TryParse(configuration.AppSettings.Settings["timezoneDifference"].Value, out timezoneDifference)) {
				//dateTime.AddHours(timezoneDifference);
				Response.Write(timezoneDifference + "<br>");
			}
		}

		Response.Write(DateTime.Now + "<br>");

		Response.Write(DateTime.Now.AddHours(3) + "<br>");
	}
}
