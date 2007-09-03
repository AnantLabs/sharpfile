using System;
using FlickrNet;
using System.Web.Configuration;
using System.Collections.Generic;

/// <summary>
/// Grabs all of the photos from flickr with the appropriate tags.
/// </summary>
public class Photos : List<Photo> {
	public Photos() {
		Flickr flickr = new Flickr();

		PhotoSearchOptions options = new PhotoSearchOptions();
		options.UserId = WebConfigurationManager.AppSettings["flickrUid"];
		options.PerPage = int.Parse(WebConfigurationManager.AppSettings["flickrPerPage"]);
		options.Tags = WebConfigurationManager.AppSettings["flickrTag"];
		options.SortOrder = PhotoSearchSortOrder.DatePostedDesc;

		FlickrNet.Photo[] photoArray = flickr.PhotosSearch(options).PhotoCollection;

		foreach (FlickrNet.Photo flickrPhoto in photoArray) {
			PhotoInfo photoInfo = flickr.PhotosGetInfo(flickrPhoto.PhotoId);
			Photo photo = new Photo(flickrPhoto.PhotoId, flickrPhoto.Title, photoInfo.Description,
				flickrPhoto.ThumbnailUrl, flickrPhoto.MediumUrl, flickrPhoto.LargeUrl);

			this.Add(photo);
		}
	}
}