using System;
using System.Web.UI;

public partial class Controls_HillelliesEntry : UserControl {
	private int userId;
	private string title;
	private string imageUrl;

	protected void Page_Load(object sender, EventArgs e) {
		rptContent.DataSource = TheHillellies.GetPosts(userId);
		rptContent.DataBind();

		lblTitle.Text = title;
		imgTitle.ImageUrl = imageUrl;
	}

	public int UserId {
		get { return userId; }
		set { userId = value; }
	}

	public string Title { get { return title; } set { title = value; } }

	public string ImageUrl { get { return imageUrl; } set { imageUrl = value; } }
}