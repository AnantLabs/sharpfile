using System;
using System.Web.UI;
using Data.Blog;
using Data;
using Domain.Blog;

public partial class Controls_HillelliesEntry : UserControl {
	private string userName;
	private string title;
	private string imageUrl;

	protected void Page_Load(object sender, EventArgs e) {
		Entries entries = new Entries(new TheHillellies(), userName);

		rptContent.DataSource = entries;
		rptContent.DataBind();

		lblTitle.Text = title;
		imgTitle.ImageUrl = imageUrl;
	}

	public string UserName {
		get { return userName; }
		set { userName = value; }
	}

	public string Title { get { return title; } set { title = value; } }

	public string ImageUrl { get { return imageUrl; } set { imageUrl = value; } }
}