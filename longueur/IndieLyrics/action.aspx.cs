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
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using Common;

public partial class action : System.Web.UI.Page {
	protected void Page_Load(object sender, EventArgs e) {
		if (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) {
			if(true) { //(Request["id"] != null && ((SiteUser)Session[Constants.CurrentUser]).Guid == Request["id"]) {
				// Check the User.IsInRole() this here.
				string result = string.Empty;

				if (Request["action"] != null) {
					switch (Request["action"]) {
						case "amazon":
							if (Request["artistName"] != null || Request["albumName"] != null || Request["songName"] != null) {
								string imageUrl = string.Empty;
								string mediumImageUrl = string.Empty;
								string largeImageUrl = string.Empty;
								string artists = string.Empty;
								string albums = string.Empty;
								string tracks = string.Empty;

								string artistName = Request["artistName"] != null ? Request["artistName"] : string.Empty;
								string albumName = Request["albumName"] != null ? Request["albumName"] : string.Empty;
								string trackName = Request["songName"] != null ? Request["songName"] : string.Empty;

								if (string.IsNullOrEmpty(Request["explicit"]) || Request["explicit"].Equals("0")) {
									if (!string.IsNullOrEmpty(artistName) && !string.IsNullOrEmpty(albumName)) {
										DataSet albumResults = IndieLyricsData.AlbumGetDetails(artistName, albumName);

										if (albumResults.Tables[0].Rows.Count > 0) {
											albumResults.Tables[0].PrimaryKey = new DataColumn[] { 
												albumResults.Tables[0].Columns["ArtistName"], 
												albumResults.Tables[0].Columns["AlbumName"] };
											
											DataRow[] rows = albumResults.Tables[0].Select("ArtistName = '" + artistName + "' AND AlbumName = '" + albumName +"'");

											if (rows.Length > 0) {
												imageUrl = rows[0]["AlbumImg"].ToString();
											} else if (false) {
												foreach (DataRow row in albumResults.Tables[0].Rows) {
													if (row["ArtistName"] != null && row["ArtistName"] != Convert.DBNull && row["ArtistName"].ToString().ToLower().Equals(artistName.ToLower())
														&& row["AlbumName"] != null && row["AlbumName"] != Convert.DBNull && row["AlbumName"].ToString().ToLower().Equals(albumName.ToLower())
														&& row["AlbumImg"] != null && row["AlbumImg"] != Convert.DBNull && row["AlbumImg"].ToString() != string.Empty) {

														imageUrl = row["AlbumImg"].ToString();
													}
												}
											}

											if (!string.IsNullOrEmpty(imageUrl) && albumResults.Tables[1].Rows.Count > 0) {
												foreach (DataRow row in albumResults.Tables[1].Rows) {
													tracks += row["SongName"].ToString() + ", ";
												}

												if (tracks.EndsWith(", ")) {
													tracks = tracks.Remove(tracks.Length - 2, 2);
												}
											}
										}
									}
								}

								if ((Request["explicit"] != null && Request["explicit"].Equals("1")) || imageUrl == string.Empty) {
									//i wonder if other things (other than spaces) need to be replaced

									//The old way to generate AmazonInfo (manually parsed the XML)
									//Uri uri = new Uri(string.Format("http://webservices.amazon.com/onca/xml?Service=AWSECommerceService&AWSAccessKeyId={0}&Operation=ItemSearch&Artist={1}&Album={2}&SearchIndex=Music&ResponseGroup=Images,ItemAttributes,Tracks",
									//    accessKey,
									//    artistName.Replace(" ", "%20"),
									//    albumName.Replace(" ", "%20")));

									//AmazonInfo ai = new AmazonInfo(uri);

									Hashtable settings = new Hashtable();
									if (!string.IsNullOrEmpty(artistName)) {
										settings["artist"] = artistName;
									}
									if (!string.IsNullOrEmpty(albumName)) {
										settings["album"] = albumName;
									}
									if (!string.IsNullOrEmpty(trackName))
									{
										settings["track"] = trackName;
									}

									AmazonInfo ai = new AmazonInfo(settings);

									if (ai.GetArtists().Count > 1 || !(ai.GetArtists().Count == 1 && ai.GetArtists()[0].ToString().ToLower().Equals(artistName.ToLower()))) {
										artists = ReturnHyperlinkedCommaString(ai.GetArtists(), "getArtistTextBoxId()");
									}

									if (ai.ContainsArtist(artistName)) {
										albums = ReturnHyperlinkedCommaString(ai.GetAlbums(artistName), "getAlbumTextBoxId()");
									} else {
										albums = ReturnHyperlinkedCommaString(ai.GetAlbums(), "getAlbumTextBoxId()");
									}

									if (ai.ContainsAlbum(artistName.Replace("%20", " "), albumName.Replace("%20", " "))) {
										imageUrl = ai.GetImage(artistName, albumName, AmazonInfo.ImageType.SmallImage);
										tracks = ReturnHyperlinkedCommaString(ai.GetTracks(artistName.Replace("%20", " "), albumName.Replace("%20", " ")), "getSongTextBoxId()");
									} else {
										tracks = ReturnHyperlinkedCommaString(ai.GetTracks(), "getSongTextBoxId()");
									}
								}

								if (!string.IsNullOrEmpty(imageUrl) /* || is the amazon no image picture */ ) {
									//update the database w/ new url
									//and return img with url in src
									result = string.Format("amazonImg|<img src=\"{0}\" width\"75\" height=\"75\" alt=\"{1}\">",
										imageUrl,
										artistName + " - " + albumName);
								}

								if (string.IsNullOrEmpty(result)) {
									result = "amazonImg|<img src=\"images/no_img.gif\" width=\"75\" height=\"75\" alt=\"No Image Available\" />";
								}

								if (!string.IsNullOrEmpty(artists))
								{
									result += "|amazonArtists|" + artists;
								}

								if (!string.IsNullOrEmpty(albums)) {
									result += "|amazonAlbums|" + albums;
								}

								if (!string.IsNullOrEmpty(tracks)) {
									result += "|amazonTracks|" + tracks;
								}
							}

							break;
						default:
							result = string.Empty;
							break;
					}
				}

				Page.Controls.Add(new LiteralControl(result));
			}
		}
	}

	public string ReturnHyperlinkedString(string item, string id) {
		return "<a href=\"#\" onclick=\"document.getElementById(" + id + ").value = '" + item + "'\">" + item + "</a>";
	}

	public string ReturnHyperlinkedCommaString(ArrayList list, string id) {
		StringBuilder retVal = new StringBuilder();

		foreach (object item in list) {
			//retVal.Append("<a href=\"#\" onclick=\"document.getElementById(" + id + ").value = '" + item.ToString() + "'\">" + item.ToString() + "</a>, ");
			retVal.Append(ReturnHyperlinkedString(item.ToString(), id) + ", ");
		}

		if (retVal.ToString().EndsWith(", ")) {
			return retVal.ToString(0, retVal.Length - 2).Trim();
		} else {
			return retVal.ToString().Trim();
		}
	}

	public string ReturnHyperlinkedCommaString(DataTable table, string column, string id) {
		StringBuilder retVal = new StringBuilder();

		foreach (DataRow row in table.Rows) {
			//retVal.Append("<a href=\"#\" onclick=\"document.getElementById(" + id + ").value = '" + item.ToString() + "'\">" + item.ToString() + "</a>, ");
			retVal.Append(ReturnHyperlinkedString(row[column].ToString(), id) + ", ");
		}

		if (retVal.ToString().EndsWith(", ")) {
			return retVal.ToString(0, retVal.Length - 2).Trim();
		} else {
			return retVal.ToString().Trim();
		}
	}
}
