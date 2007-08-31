using System;
using FlickrNet;
using System.Collections.Generic;

public partial class Photos_Default : System.Web.UI.Page {
	protected void Page_Load(object sender, EventArgs e) {
		if (!IsPostBack) {
			Flickr flickr = new Flickr();

			PhotoSearchOptions options = new PhotoSearchOptions();
			options.UserId = "10086551@N05";
			options.PerPage = 100;
			options.Tags = "Phlog";
			options.SortOrder = PhotoSearchSortOrder.DatePostedDesc;
			Photos photos = flickr.PhotosSearch(options);

			rptPhotos.DataSource = photos.PhotoCollection;
			rptPhotos.DataBind();

			if (photos.PhotoCollection.GetLength(0) > 0) {
				imgCurrent.ImageUrl = photos.PhotoCollection[0].MediumUrl;
			}
		}

		//photos.PhotoCollection[0].Des

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
}