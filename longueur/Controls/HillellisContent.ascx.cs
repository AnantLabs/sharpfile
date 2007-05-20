using System;
using System.Web.UI;
using Data.Blog;
using Domain.Blog;
using Data;

public partial class Controls_HillellisEntry : UserControl {
	private string userName;
	private string titleImageUrl;
	private string titleImageTooltip;
	private string titleImageAlternativeText;

	private Entries entries;
	private int lastEntryId = 0;

	protected void Page_Load(object sender, EventArgs e) {
		entries = new Entries(new TheHillellis(), userName);

		rptContent.DataSource = entries;
		rptContent.DataBind();

		imgTitle.ImageUrl = titleImageUrl;

		if (!string.IsNullOrEmpty(titleImageAlternativeText)) {
			imgTitle.AlternateText = titleImageAlternativeText;
		}

		if (!string.IsNullOrEmpty(titleImageTooltip)) {
			imgTitle.ToolTip = titleImageTooltip;
		}
	}

	protected int getLastId() {
		if (lastEntryId == 0) {
			lastEntryId = entries[entries.Count-1].Id;
		}

		return lastEntryId;
	}

	public string UserName {
		get { return userName; }
		set { userName = value; }
	}

	public string TitleImageUrl { 
		get { return titleImageUrl; } 
		set { titleImageUrl = value; } 
	}

	public string TitleImageTooltip { 
		get { return titleImageTooltip; } 
		set { titleImageTooltip = value; } 
	}

	public string TitleImageAlternativeText { 
		get { return titleImageAlternativeText; } 
		set { titleImageAlternativeText = value; } 
	}
}