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
using Common;

public partial class delete_user : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) {
			IndieLyricsData.DeleteUser(((SiteUser)Session[Constants.CurrentUser]).Id);
		}

		Response.Clear();
		Response.Redirect("default.aspx", true);
    }
}
