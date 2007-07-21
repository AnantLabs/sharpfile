using System;

public partial class TheHillellis_Permalink : System.Web.UI.Page {
	protected void Page_Load(object sender, EventArgs e) {
		if (ctlContent != null) {
			if (Request.QueryString["id"] != null) {
				int id = 0;

				if (int.TryParse(Request["id"], out id)) {
					if (id > 0) {
						this.ctlContent.EntryId = id;
					}
				}
			}
		}
	}
}
