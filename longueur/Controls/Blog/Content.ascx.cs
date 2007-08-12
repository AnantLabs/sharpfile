using System;
using System.Web.UI;
using Data.Blog;
using Domain.Blog;
using Data;
using System.Web.UI.WebControls;

public partial class Controls_HillellisEntry : UserControl {
	private string userName;
	private string titleImageUrl;
	private string titleImageTooltip;
	private string titleImageAlternativeText;

	// Used for permalink entries.
	private int entryId = 0;

	// Used for archive entries.
	private int archiveId = 0;

	private BaseEntries entries;
	private int lastEntryId = 0;

	protected void Page_Load(object sender, EventArgs e) {
		TheHillellis theHillellisData = new TheHillellis();
		ThemeType themeType = ThemeType.Spring;

		if (this.Request.Cookies["TheHillellis"] != null &&
			!string.IsNullOrEmpty(this.Request.Cookies["TheHillellis"]["Theme"])) {
			themeType = (ThemeType)Enum.Parse(typeof(ThemeType), this.Request.Cookies["TheHillellis"]["Theme"]);
		}

		if (entryId > 0 || 
			archiveId > 0) {
			string elementId = string.Empty;

			if (entryId > 0) {
				entries = EntriesFactory.GetEntries(themeType, theHillellisData, entryId);
				elementId = "permalinkContent";
			} else if (archiveId > 0) {
				entries = EntriesFactory.GetArchiveEntries(themeType, theHillellisData, archiveId);
				elementId = "archiveContent";
			}

			string backgroundColor = entries.BackgroundColor;
			titleImageUrl = entries.TitleImageUrl;

			this.Page.ClientScript.RegisterStartupScript(typeof(string), "changeBackgroundColor", "changeBackgroundColor('" + elementId + "', '" + backgroundColor + "');", true);
		} else if (!string.IsNullOrEmpty(userName)) {
			entries = EntriesFactory.GetEntries(themeType, theHillellisData, userName);
		} else {
			rptContent.Visible = false;
			phTopHat.Visible = false;

			lblMessage.Text = "Look like something is invalid.<br />Maybe you should try again?";
			lblMessage.Visible = true;
		}

		rptContent.DataSource = entries;
		rptContent.DataBind();

		if (string.IsNullOrEmpty(titleImageUrl)) {
			imgTitle.Visible = false; 
		} else {
			imgTitle.ImageUrl = titleImageUrl;

			if (!string.IsNullOrEmpty(titleImageAlternativeText)) {
				imgTitle.AlternateText = titleImageAlternativeText;
			}

			if (!string.IsNullOrEmpty(titleImageTooltip)) {
				imgTitle.ToolTip = titleImageTooltip;
			}
		}
	}

	protected int getLastId() {
		if (lastEntryId == 0) {
			lastEntryId = entries[entries.Count-1].Id;
		}

		return lastEntryId;
	}

	protected override void OnInit(EventArgs e) {
		InitializeComponents();
		base.OnInit(e);
	}

	private void InitializeComponents() {
		this.rptContent.ItemDataBound += new RepeaterItemEventHandler(rptContent_ItemDataBound);
	}

	void rptContent_ItemDataBound(object sender, RepeaterItemEventArgs e) {
		RepeaterItem item = e.Item;

		if (item.ItemType == ListItemType.AlternatingItem ||
			item.ItemType == ListItemType.Item) {
			Repeater rptTags = (Repeater)item.FindControl("rptTags");

			rptTags.DataSource = ((Entry)item.DataItem).Tags;
			rptTags.DataBind();
		}
	}

	public int EntryId {
		get { return entryId; }
		set { entryId = value; }
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

	public int ArchiveId { 
		get { return archiveId; } 
		set { archiveId = value; } 
	}
}