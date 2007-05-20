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

public partial class TheHillellis_Default : System.Web.UI.Page {
	protected void Page_Load(object sender, EventArgs e) {
		// TODO: Randomize this bitch.
		this.Title = "The Hillellis: The Final Countdown.";

		this.ctlLeftContent.UserName="lynn";
		this.ctlLeftContent.TitleImageUrl = "~/TheHillellis/Images/cupcake_t.png";
		this.ctlLeftContent.TitleImageTooltip = "Lynn says funny things.";
		this.ctlLeftContent.TitleImageAlternativeText = "";

		this.ctlRightContent.UserName="adam";
		this.ctlRightContent.TitleImageUrl = "~/TheHillellis/Images/Puppup_t.png";
		this.ctlRightContent.TitleImageTooltip = "Adam sometimes listens.";
		this.ctlRightContent.TitleImageAlternativeText = "";
	}
}
