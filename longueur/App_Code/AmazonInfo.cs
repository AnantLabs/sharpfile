using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using Amazon;
using Common;

/// <summary>
/// Summary description for AmazonInfo
/// </summary>
public class AmazonInfo {
	private HybridDictionary artistAlbumHash = new HybridDictionary(true);
	private HybridDictionary albumTrackHash = new HybridDictionary(true);
	private HybridDictionary albumSmallImgHash = new HybridDictionary(true);
	private HybridDictionary albumMediumImgHash = new HybridDictionary(true);
	private HybridDictionary albumLargeImgHash = new HybridDictionary(true);

	private int albumCount = 0;
	private int trackCount = 0;

	public AmazonInfo() {
	}

	public AmazonInfo(Hashtable settings) {
		//StreamWriter sw = new StreamWriter(@"c:\amazonInfo.txt", false);
		AWSECommerceService aws = new AWSECommerceService();
		ItemSearchRequest request = new ItemSearchRequest();

		if (settings.Contains("artist")) {
			//request.Artist = "Aesop Rock";
			request.Artist = settings["artist"].ToString();
		}
		if (settings.Contains("title")) {
			//request.Title = "Labor Days";
			request.Title = settings["title"].ToString();
		}
		if (settings.Contains("track")) {
			request.Keywords = settings["track"].ToString();
		}

		request.SearchIndex = "Music";
		request.ResponseGroup = new string[] { "Medium", "Tracks" };
		request.Sort = "salesrank";

		ItemSearchRequest[] requests = new ItemSearchRequest[] { request };

		ItemSearch itemSearch = new ItemSearch();
		//itemSearch.AssociateTag = "myassociatetag-20";
		itemSearch.SubscriptionId = "0HXEDBNR2CBQNATN2382";
		itemSearch.Request = requests;

		try {
			ItemSearchResponse response = aws.ItemSearch(itemSearch);

			if (response.Items[0] != null) {
				Items info = response.Items[0];
				Item[] items = info.Item;

				if (items != null) {
					for (int i = 0; i < items.Length; i++) {
						if (items[i] != null) {
							Item item = items[i];

							string itemArtist = string.Empty;
							string smallImageUrl = string.Empty;
							string mediumImageUrl = string.Empty;
							string largeImageUrl = string.Empty;
							string itemAlbum = string.Empty;

							if (item.ItemAttributes.Artist != null) {
								//for (int j = 0; j < item.ItemAttributes.Artist.Length; j++) {
									itemArtist = item.ItemAttributes.Artist[0];
									AddArtist(item.ItemAttributes.Artist[0]);
								//}
							}

							if (item.SmallImage != null && item.SmallImage.URL != null) {
								smallImageUrl = item.SmallImage.URL;
							}
							if (item.MediumImage != null && item.MediumImage.URL != null) {
								mediumImageUrl = item.MediumImage.URL;
							}
							if (item.LargeImage != null && item.LargeImage.URL != null) {
								largeImageUrl = item.LargeImage.URL;
							}

							if (item.ItemAttributes.Title != null) {
								itemAlbum = item.ItemAttributes.Title;
							}

							if (item.ASIN != null) {
								//not use presently
								//Console.WriteLine("ASIN: " + item.ASIN);
							}

							if (item.DetailPageURL != null) {
								//not used presently
								//Console.WriteLine("Detail URL: " + item.DetailPageURL);
							}

							if (item.ItemAttributes.Format != null) {
								for (int j = 0; j < item.ItemAttributes.Format.Length; j++) {
									//not used presently
									//Console.WriteLine("Format: " + item.ItemAttributes.Format[j]);
								}
							}

							if (!string.IsNullOrEmpty(itemArtist)
								&& !string.IsNullOrEmpty(itemAlbum)
								&& !ContainsAlbum(itemArtist, itemAlbum)
								&& (!string.IsNullOrEmpty(smallImageUrl)
								|| !string.IsNullOrEmpty(mediumImageUrl)
								|| !string.IsNullOrEmpty(largeImageUrl))) {

								string smallImage = smallImageUrl == string.Empty ? "" : SaveImage(itemArtist, itemAlbum, smallImageUrl, ImageType.SmallImage);
								string mediumImage = mediumImageUrl == string.Empty ? "" : SaveImage(itemArtist, itemAlbum, mediumImageUrl, ImageType.MediumImage);
								string largeImage = largeImageUrl == string.Empty ? "" : SaveImage(itemArtist, itemAlbum, largeImageUrl, ImageType.LargeImage);

								AddAlbum(itemArtist, itemAlbum, smallImage, mediumImage, largeImage);
								//sw.WriteLine("adding artist, album, images");
							} else if (!string.IsNullOrEmpty(itemArtist)
								&& !string.IsNullOrEmpty(itemAlbum)
								&& !ContainsAlbum(itemArtist, itemAlbum)) {

								AddAlbum(itemArtist, itemAlbum);
								//sw.WriteLine("adding artist, album");
							} else if (!string.IsNullOrEmpty(itemArtist)
								&& !ContainsArtist(itemArtist)) {

								AddArtist(itemArtist);
								//sw.WriteLine("adding artist");
							}

							if (ContainsAlbum(itemArtist, itemAlbum)) {
								if (item.Tracks != null) {
									for (int j = 0; j < item.Tracks.Length; j++) {
										if (item.Tracks[j].Track != null) {
											for (int k = 0; k < item.Tracks[j].Track.Length; k++) {
												if (!ContainsTrack(itemArtist, itemAlbum, item.Tracks[j].Track[k].Value)) {
													AddTrack(itemArtist, itemAlbum, item.Tracks[j].Track[k].Value);
												}
											}
										}
									}

									//sw.WriteLine("adding tracks");
								}
							}

							//sw.WriteLine(Environment.NewLine);
						}
					}
				}
			}
		} catch (Exception ex) {
			//Console.WriteLine(ex.ToString());
			//sw.WriteLine(ex.Message + ex.StackTrace);
		}

		//sw.Flush();
		//sw.Close();
	}

	[Obsolete("Manually parses the XML, so this might be a bad idea to use")]
	public AmazonInfo(Uri uri) {
		XmlReaderSettings settings = new XmlReaderSettings();
		settings.IgnoreWhitespace = true;

		StreamWriter sw = new StreamWriter(@"c:\amazonInfo.txt", false);

		using (XmlReader xmlReader = XmlReader.Create(uri.ToString(), settings)) {
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlReader);
			XmlElement root = doc.DocumentElement;

			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("am", "http://webservices.amazon.com/AWSECommerceService/2005-10-05");
			XmlNodeList nodes = root.SelectNodes("//am:Item", nsmgr);

			foreach (XmlNode node in nodes) {
				XmlDocument doc2 = new XmlDocument();
				doc2.LoadXml(node.OuterXml);

				string smallImageUrl = doc2.SelectSingleNode("//am:SmallImage", nsmgr) == null ? "" : doc2.SelectSingleNode("//am:SmallImage", nsmgr).ChildNodes[0].InnerText;
				sw.WriteLine("sm: " + smallImageUrl);
				string mediumImageUrl = doc2.SelectSingleNode("//am:MediumImage", nsmgr) == null ? "" : doc2.SelectSingleNode("//am:MediumImage", nsmgr).ChildNodes[0].InnerText;
				sw.WriteLine("med: " + mediumImageUrl);
				string largeImageUrl = doc2.SelectSingleNode("//am:LargeImage", nsmgr) == null ? "" : doc2.SelectSingleNode("//am:LargeImage", nsmgr).ChildNodes[0].InnerText;
				sw.WriteLine("lrg: " + largeImageUrl);

				string itemArtist = doc2.SelectSingleNode("//am:Artist", nsmgr) == null ? "" : doc2.SelectSingleNode("//am:Artist", nsmgr).InnerText;
				sw.WriteLine("artist: " + itemArtist);

				string itemAlbum = doc2.SelectSingleNode("//am:Title", nsmgr) == null ? "" : doc2.SelectSingleNode("//am:Title", nsmgr).InnerText;
				sw.WriteLine("album: " + itemAlbum);

				string itemUrl = doc2.SelectSingleNode("//am:DetailPageURL", nsmgr) == null ? "" : doc2.SelectSingleNode("//am:DetailPageURL", nsmgr).InnerText;

				if (!string.IsNullOrEmpty(itemArtist)
					&& !string.IsNullOrEmpty(itemAlbum)
					&& !ContainsAlbum(itemArtist, itemAlbum)
					&& (!string.IsNullOrEmpty(smallImageUrl)
					|| !string.IsNullOrEmpty(mediumImageUrl)
					|| !string.IsNullOrEmpty(largeImageUrl)))
				{
					string smallImage = smallImageUrl == string.Empty ? "" : SaveImage(itemArtist, itemAlbum, smallImageUrl, ImageType.SmallImage);
					string mediumImage = mediumImageUrl == string.Empty ? "" : SaveImage(itemArtist, itemAlbum, mediumImageUrl, ImageType.MediumImage);
					string largeImage = largeImageUrl == string.Empty ? "" : SaveImage(itemArtist, itemAlbum, largeImageUrl, ImageType.LargeImage);

					AddAlbum(itemArtist, itemAlbum, smallImage, mediumImage, largeImage);
					sw.WriteLine("adding artist, album, images");
				} else if (!string.IsNullOrEmpty(itemArtist)
					&& !string.IsNullOrEmpty(itemAlbum)
					&& !ContainsAlbum(itemArtist, itemAlbum)) {

					AddAlbum(itemArtist, itemAlbum);
					sw.WriteLine("adding artist, album");
				} else if (!string.IsNullOrEmpty(itemArtist)
					&& !ContainsArtist(itemArtist)) {
					
					AddArtist(itemArtist);
					sw.WriteLine("adding artist");
				}

				if (ContainsAlbum(itemArtist, itemAlbum)) {
					foreach (XmlNode trackNode in doc2.SelectNodes("//am:Track", nsmgr)) {
						if (!ContainsTrack(itemArtist, itemAlbum, trackNode.InnerText)) {
							AddTrack(itemArtist, itemAlbum, trackNode.InnerText);
						}
					}

					sw.WriteLine("adding tracks");
				}

				sw.WriteLine(Environment.NewLine);
			}
		}

		sw.Flush();
		sw.Close();
	}

	public AmazonInfo(string connectionString) {
		//populate AI from database
	}

	public void AddArtist(string artistName) {
		if (!artistAlbumHash.Contains(artistName)) {
			artistAlbumHash.Add(artistName, new CaseInsensitiveArrayList());
		}
	}

	public void AddAlbum(string artistName, string albumName) {
		AddArtist(artistName);

		if (!((CaseInsensitiveArrayList)artistAlbumHash[artistName]).Contains(albumName)) {
			((CaseInsensitiveArrayList)artistAlbumHash[artistName]).Add(albumName);
			albumCount += 1;
		}

		if (!albumTrackHash.Contains(Concatanate(artistName, albumName))) {
			albumTrackHash.Add(Concatanate(artistName, albumName), new ArrayList());
		}
	}

	public void AddAlbum(string artistName, string albumName, string smallImg) {
		AddAlbum(artistName, albumName, smallImg, "", "");
	}

	public void AddAlbum(string artistName, string albumName, string smallImg, string medImg) {
		AddAlbum(artistName, albumName, smallImg, medImg, "");
	}

	public void AddAlbum(string artistName, string albumName, string smallImg, string medImg, string largeImg) {
		AddArtist(artistName);

		if (!((CaseInsensitiveArrayList)artistAlbumHash[artistName]).Contains(albumName)) {
			((CaseInsensitiveArrayList)artistAlbumHash[artistName]).Add(albumName);
			albumCount += 1;
		}

		if (!albumTrackHash.Contains(Concatanate(artistName, albumName))) {
			albumTrackHash.Add(Concatanate(artistName, albumName), new ArrayList());
		}

		if (!string.IsNullOrEmpty(smallImg) && !albumSmallImgHash.Contains(Concatanate(artistName, albumName))) {
			albumSmallImgHash.Add(Concatanate(artistName, albumName), smallImg);
			//Data.InsertAlbumCover();
		}

		if (!string.IsNullOrEmpty(medImg) && !albumMediumImgHash.Contains(Concatanate(artistName, albumName))) {
			albumMediumImgHash.Add(Concatanate(artistName, albumName), medImg);
		}

		if (!string.IsNullOrEmpty(largeImg) && !albumLargeImgHash.Contains(Concatanate(artistName, albumName))) {
			albumLargeImgHash.Add(Concatanate(artistName, albumName), largeImg);
		}
	}

	public void AddTrack(string artistName, string albumName, string trackName) {
		AddAlbum(artistName, albumName);

		if (albumTrackHash != null && !ContainsTrack(artistName, albumName, trackName)) {
			((ArrayList)albumTrackHash[Concatanate(artistName, albumName)]).Add(trackName);
			trackCount += 1;
		}
	}

	public bool ContainsArtist(string artistName) {
		if (artistAlbumHash.Contains(artistName)) {
			return true;
		}

		return false;
	}

	public bool ContainsAlbum(string albumName) {
		if (artistAlbumHash != null) {
			foreach (string key in artistAlbumHash.Keys) {
				if (((CaseInsensitiveArrayList)artistAlbumHash[key]).Contains(albumName)) {
					return true;
				}
			}
		}

		return false;
	}

	public bool ContainsAlbum(string artistName, string albumName) {
		if (artistAlbumHash != null &&
			artistAlbumHash.Contains(artistName) &&
			((CaseInsensitiveArrayList)artistAlbumHash[artistName]).Contains(albumName)) {
			return true;
		}

		return false;
	}

	public bool ContainsTrack(string trackName) {
		if (albumTrackHash != null) {
			foreach (string key in albumTrackHash.Keys) {
				foreach (string s in (ArrayList)albumTrackHash[key]) {
					if (s.Equals(trackName)) {
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool ContainsTrack(string artistName, string trackName) {
		if (albumTrackHash != null) {
			foreach (string key in albumTrackHash.Keys) {
				if (key.StartsWith(artistName)) {
					foreach (string s in (ArrayList)albumTrackHash[key]) {
						if (s.Equals(trackName)) {
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	public bool ContainsTrack(string artistName, string albumName, string trackName) {
		if (albumTrackHash != null &&
			albumTrackHash.Contains(Concatanate(artistName, albumName)) &&
			((ArrayList)albumTrackHash[Concatanate(artistName, albumName)]).Contains(trackName)) {
			return true;
		}

		return false;
	}

	public ArrayList GetTracks() {
		ArrayList trackList = new ArrayList();

		if (albumTrackHash != null) {
			foreach (string artistAlbum in albumTrackHash.Keys) {
				trackList.AddRange((ArrayList)albumTrackHash[artistAlbum]);
			}
		}

		return trackList;
	}

	public ArrayList GetTracks(string artistName) {
		ArrayList trackList = new ArrayList();

		if (albumTrackHash != null) {
			foreach (string artistAlbum in albumTrackHash.Keys) {
				if (artistAlbum.StartsWith(artistName)) {
					trackList.AddRange((ArrayList)albumTrackHash[artistAlbum]);
				}
			}
		}

		return trackList;
	}

	public ArrayList GetTracks(string artistName, string albumName) {
		if (albumTrackHash != null &&
			albumTrackHash.Contains(Concatanate(artistName, albumName))) {
			return (ArrayList)albumTrackHash[Concatanate(artistName, albumName)];
		}

		return new ArrayList();
	}

	public string GetImage(string artistName, string albumName, ImageType imageType) {
		string image = string.Empty;

		if (ContainsAlbum(artistName, albumName)) {
			if (imageType == ImageType.SmallImage) {
				if (albumSmallImgHash.Contains(Concatanate(artistName, albumName))) {
					image = albumSmallImgHash[Concatanate(artistName, albumName)].ToString();
				}
			}
		}

		return image;
	}

	public ArrayList GetAlbums(string artistName) {
		if (artistAlbumHash != null &&
			artistAlbumHash.Contains(artistName)) {
			return (ArrayList)artistAlbumHash[artistName];
		}

		return new ArrayList();
	}

	public ArrayList GetAlbums() {
		if (artistAlbumHash != null) {
			ArrayList albums = new ArrayList();

			foreach (string key in artistAlbumHash.Keys) {
				albums.AddRange((ArrayList)artistAlbumHash[key]);
			}

			return albums;
		}

		return new ArrayList();
	}

	public ArrayList GetArtists() {
		return new ArrayList(artistAlbumHash.Keys);
	}

	public int ArtistCount() {
		return artistAlbumHash.Keys.Count;
	}

	public int AlbumCount() {
		return albumCount;
	}

	public int AlbumCount(string artistName) {
		int c = 0;

		if (artistAlbumHash != null) {
			foreach (string key in artistAlbumHash.Keys) {
				if (key.StartsWith(artistName)) {
					c += 1;
				}
			}
		}

		return c;
	}

	public int TrackCount() {
		return trackCount;
	}

	public int TrackCount(string artistName, string albumName) {
		if (albumTrackHash != null 
			&& albumTrackHash.Contains(Concatanate(artistName, albumName))) {
			return ((ArrayList)albumTrackHash[Concatanate(artistName, albumName)]).Count;
		}

		return 0;
	}

	public int TrackCount(string artistName) {
		if (albumTrackHash != null) {
			foreach (string key in albumTrackHash.Keys) {
				if (key.StartsWith(artistName)) {
					return ((ArrayList)albumTrackHash[key]).Count;
				}
			}
		}

		

		return 0;
	}

	public enum ImageType {
		SmallImage,
		MediumImage,
		LargeImage
	}
	
	private string Concatanate(string artistName, string albumName) {
		return artistName + " - " + albumName;
	}

	private string SaveImage(string artistName, string albumName, string imageUrl, ImageType type) {
		string virtualPath = HttpContext.Current.Request.FilePath;
		virtualPath = virtualPath.Substring(0, virtualPath.LastIndexOf(@"/")) + @"/images/albumcovers/";
		
		string physicalPath = HttpContext.Current.Request.MapPath(HttpContext.Current.Request.FilePath);
		physicalPath = physicalPath.Substring(0, physicalPath.LastIndexOf(@"\")) + @"\images\albumcovers\";

		if (!Directory.Exists(physicalPath)) {
			Directory.CreateDirectory(physicalPath);
		}

		Regex regex = new Regex(@"[^a-zA-Z0-9]");

		string imgType = string.Empty;
		string ext = General.GetExt(imageUrl);

		if (type == ImageType.SmallImage) {
			imgType = "_s";
		} else if (type == ImageType.MediumImage) {
			imgType = "_m";
		} else if (type == ImageType.LargeImage) {
			imgType = "_l";
		}

		physicalPath = physicalPath + regex.Replace(artistName, "_") + "-" + regex.Replace(albumName, "_") + imgType + ext;
		virtualPath = virtualPath + regex.Replace(artistName, "_") + "-" + regex.Replace(albumName, "_") + imgType + ext;

		if (Common.HttpUtility.GetHttpResponseImage(imageUrl, physicalPath)) {
			return virtualPath;
		}

		return string.Empty;
	}

	//private ArrayList GetTracksFromXml(XmlReader reader) {
	//    ArrayList trackList = new ArrayList();
	//    reader.ReadToFollowing("Disc");

	//    using (XmlReader trackReader = reader.ReadSubtree()) {
	//        while (!trackReader.EOF) {
	//            trackReader.ReadToFollowing("Track");

	//            string track = trackReader.ReadString();
	//            if (track != null && track != string.Empty) {
	//                trackList.Add(track);
	//            }
	//        }
	//    }

	//    return trackList;
	//}
}
