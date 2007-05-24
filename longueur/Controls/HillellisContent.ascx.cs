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
	private int entryId = 0;

	private Entries entries;
	private int lastEntryId = 0;

	protected void Page_Load(object sender, EventArgs e) {
		TheHillellis theHillellisData = new TheHillellis();

		if (entryId > 0) {
			entries = new Entries();
			Entry entry = new Entry(theHillellisData, entryId);

			if (entry.Name == "lynn") {
				titleImageUrl = "~/TheHillellis/Images/cupcake_t.png";
			} else if (entry.Name == "adam") {
				titleImageUrl = "~/TheHillellis/Images/puppup_t.png";
			}

			entries.Add(entry);
		} else if (!string.IsNullOrEmpty(userName)) {
			entries = new Entries(theHillellisData, userName);
		} else {
			entries = new Entries();
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
}