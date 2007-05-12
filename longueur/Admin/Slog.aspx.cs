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

public partial class Admin_Slog : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}

	protected override void OnInit(EventArgs e)
	{
		InitializeComponent();
		base.OnInit(e);
	}

	private void InitializeComponent()
	{
		this.btnSubmit.Click += new EventHandler(btnSubmit_Click);
	}

	void btnSubmit_Click(object sender, EventArgs e)
	{
		int userId = 0;

		if (User.Identity.IsAuthenticated)
		{
			userId = int.Parse(((FormsIdentity)HttpContext.Current.User.Identity).Name);
		}

		SlogData.InsertSlog(txtTitle.Text, txtContent.Text, userId);
	}
}
