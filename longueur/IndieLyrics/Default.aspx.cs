using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Common;

public partial class _default : System.Web.UI.Page {
	protected void Page_Load(object sender, System.EventArgs e) {
		JsMsg.Text = "<!-- JavaScript has not been checked -->";

		if (!((SiteUser)Session[Constants.CurrentUser]).EnableJs) {
			ClientScript.RegisterClientScriptBlock(typeof(string), "checkJs", @"
<!--
	window.location.href = 'checkJs.aspx';
//-->
", true);
		}

		if (Request.Browser.EcmaScriptVersion.Major < 1 || !((SiteUser)Session[Constants.CurrentUser]).EnableJs) {
			JsMsg.Text = "<!-- JavaScript is not enabled --><strong>NOTE: It seems that you do not have JavaScript enabled. You should know that the site is really awful without JavaScript.  I mean even worse then normal.  You can't do stuff, really.  Just so you know.</strong><br /><br />";
		} else {
			JsMsg.Text = "<!-- JavaScript is enabled -->";
		}
	}

	#region Web Form Designer generated code
	override protected void OnInit(EventArgs e) {
		InitializeComponent();
		base.OnInit(e);
	}

	private void InitializeComponent() {

	}
	#endregion
}