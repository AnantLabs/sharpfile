using System;
using FlickrNet;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Web.UI.HtmlControls;

public partial class Photos_Default : System.Web.UI.Page {
	private Photos photos;

	protected void Page_Load(object sender, EventArgs e) {
		if (!IsPostBack) {
			rptPhotos.DataSource = photos;
			rptPhotos.DataBind();

			if (photos.Count > 0) {
				string photoId = photos[0].Id;
				loadPhoto(photoId);
			}
		}

		/*
		// PhotoSets collection don't contain all of the photos for some reason.
		Photosets photosets = flickr.PhotosetsGetList("10086551@N05");
		foreach (Photoset photoset in photosets.PhotosetCollection) {
			if (photoset.Title.Equals("Phlog")) {
				rptPhotos.DataSource = photoset.PhotoCollection;
				rptPhotos.DataBind();
			}
		}
		 */
	}

	#region Helper Methods
	private void loadPhotos() {
		if (Cache["flickrResults"] == null ||
			((Photos)Cache["flickrResults"]).Count == 0) {
			photos = new Photos();

			Cache.Insert("flickrResults", photos, null, 
				System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0), 
				System.Web.Caching.CacheItemPriority.Normal, null);
		}

		photos = (Photos)Cache["flickrResults"];
	}

	private void loadPhoto(string photoId) {
		Photo photo = photos.Find(delegate(Photo p) {
			return p.Id == photoId;
		});

		if (photo != null) {
			imgCurrent.ImageUrl = photo.MediumUrl;
			hypCurrent.NavigateUrl = photo.LargeUrl;
			ltlTitle.Text = photo.Title;
			ltlDescription.Text = photo.Description;
			//divDescription.Visible = !string.IsNullOrEmpty(photo.Description);

			int photoIndex = photos.IndexOf(photo);
			lnkNext.Enabled = !(photoIndex == (photos.Count - 1)); //photos[photoIndex + 1] != null
			lnkPrevious.Enabled = !(photoIndex == 0);

			if (lnkNext.Enabled) {
				lnkNext.CommandArgument = photos[photoIndex + 1].Id;
			}			

			if (lnkPrevious.Enabled) {
				lnkPrevious.CommandArgument = photos[photoIndex - 1].Id;
			}

			foreach (RepeaterItem item in rptPhotos.Items) {
				if (((HtmlInputHidden)item.FindControl("inpId")).Value == photoId) {
					((ImageButton)item.FindControl("imgThumbnail")).CssClass = "selectedPhoto";
				} else {
					((ImageButton)item.FindControl("imgThumbnail")).CssClass = "photo";
				}
			}
		}
	}
	#endregion

	private void InitializeComponents() {
		this.lnkNext.Click += new EventHandler(lnkNext_Click);
		this.lnkPrevious.Click += new EventHandler(lnkPrevious_Click);
	}

	#region Events
	protected void imgThumbnail_OnClick(object sender, EventArgs e) {
		string photoId = ((ImageButton)sender).CommandArgument;
		loadPhoto(photoId);		
	}

	void lnkPrevious_Click(object sender, EventArgs e) {
		string photoId = ((LinkButton)sender).CommandArgument;
		loadPhoto(photoId);
	}

	void lnkNext_Click(object sender, EventArgs e) {
		string photoId = ((LinkButton)sender).CommandArgument;
		loadPhoto(photoId);
	}
	#endregion

	#region overrides
	protected override void OnPreInit(EventArgs e) {
		loadPhotos();
		base.OnPreInit(e);
	}

	protected override void OnInit(EventArgs e) {
		InitializeComponents();
		base.OnInit(e);
	}
	#endregion
}