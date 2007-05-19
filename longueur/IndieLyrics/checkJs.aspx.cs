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
using Domain.Membership;

public partial class checkJs : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (Session[Constants.CurrentUser] != null && !((SiteUser)Session[Constants.CurrentUser]).EnableJs) {
			((SiteUser)Session[Constants.CurrentUser]).EnableJs = true;
		}

		Response.Redirect("default.aspx", true);
    }
}
