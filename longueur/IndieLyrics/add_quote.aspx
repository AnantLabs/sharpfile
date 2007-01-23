<%@ Page Language="C#" MasterPageFile="~/IndieLyrics/template.master" AutoEventWireup="true" CodeFile="add_quote.aspx.cs" Inherits="add" Title="IndieLyrics: Add a Lyric" %>
<%@ Register TagPrefix="l" TagName="Authorize" Src="~/Controls/Authorize.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">

<script language="JavaScript" type="text/javascript">
	function SetMultiTaskType(action) {
		document.forms[0].MultiTaskType.value = action;
	}
	
	function checkIfEnter(evt) {
		var key = (evt.which) ? evt.which : event.keyCode;
		
		if (key == 13) {
			SetMultiTaskType('submit');
			document.forms[0].submit();
			return false;
		} else {
			return true;
		}
	}
	
	function ToggleAmazon() {
		if (document.getElementById('amazonWorkingArea').style.display == '') {
			document.getElementById('amazonWorkingArea').style.display = 'none';
			document.getElementById('amazonExplicitCheck').style.display = '';
		} else {
			document.getElementById('amazonWorkingArea').style.display = '';
			document.getElementById('amazonExplicitCheck').style.display = 'none';
		}
		
		if (document.getElementById('amazonCheck').style.display == 'none') {
			if (document.getElementById('amazonArtists').innerHTML == '') {
				document.getElementById('amazonArtists').style.display = 'none';
			} else {
				document.getElementById('amazonArtistsRow').style.display = '';
				document.getElementById('amazonArtists').style.display = '';
				document.getElementById('amazonArtists').style.height = calculateHeight(document.getElementById('amazonArtists').innerHTML);
			}
			
			if (document.getElementById('amazonAlbums').innerHTML == '') {
				document.getElementById('amazonAlbums').style.display = 'none';
			} else {
				document.getElementById('amazonAlbumsRow').style.display = '';
				document.getElementById('amazonAlbums').style.display = '';
				document.getElementById('amazonAlbums').style.height = calculateHeight(document.getElementById('amazonAlbums').innerHTML);
			}
			
			if (document.getElementById('amazonTracks').innerHTML == '') {
				document.getElementById('amazonTracks').style.display = 'none';
			} else {
				document.getElementById('amazonTracksRow').style.display = '';
				document.getElementById('amazonTracks').style.display = '';
				document.getElementById('amazonTracks').style.height = calculateHeight(document.getElementById('amazonTracks').innerHTML);
			}
		} else {
			document.getElementById('amazonCheck').style.display = 'none';
		}
	}
	
	function calculateHeight(str) {
		if (str != null && str != '') {
			var strLength = str.replace(new RegExp(/<.*?>/g), "").length;
			
			if (strLength > 150) {
				return '50px';
			} else {
				//alert('nothing: ' + strLength);
			}
		}
		
		return '';
	}
	
	function CheckAmazonLink(explicit) {
		if ((document.getElementById(getArtistTextBoxId()) != null && document.getElementById(getArtistTextBoxId()).value != '') || 
			(document.getElementById(getAlbumTextBoxId()) != null && document.getElementById(getAlbumTextBoxId()).value != '')// || 
			//(document.getElementById(getSongTextBoxId()) != null && document.getElementById(getSongTextBoxId()).value != '')) {
			) {
			
			ToggleAmazon(); 
			sndReq('amazon', GetAmazonArgsName(explicit), ToggleAmazon);
		} else {
			alert('To check for an album cover, please type in an artist or an album.');
		}
	}
	
	function GetAmazonArgsName(explicit) {
		var args = '';
		
		if (explicit == true) {
			args += '&explicit=1';
		}
		
		if (document.getElementById(getArtistTextBoxId()).value != '') {
			args += '&artistName=' + document.getElementById(getArtistTextBoxId()).value;
		}

		if (document.getElementById(getAlbumTextBoxId()).value != '') {
			args += '&albumName=' + document.getElementById(getAlbumTextBoxId()).value;
		}
		
		//if (document.getElementById(getSongTextBoxId()).value != '') {
		//	args += '&songName=' + document.getElementById(getSongTextBoxId()).value;
		//}

		return args;
	}
</script>

<l:Authorize ID="Authorize" runat="server" Redirect="add_quote_u.aspx" UserType="User" />
<input type="hidden" name="MultiTaskType" />

<table>
	<tr>
		<td colspan="4">Add a lyric or a quote from a song.</td>
	</tr>
	<tr>
		<td colspan="4">&nbsp;</td>
	</tr>
	<tr>
		<td><strong>Artist</strong></td>
		<td>&nbsp;</td>
		<td><asp:TextBox ID="Artist" runat="server" name="Artist" Width="175"></asp:TextBox></td>
		<td style="display:none" rowspan="5" valign="top" align="center" class="border" <%= (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) ? "style=\"width: 85px; background: #DBDBDB; padding: 5px 0px 5px 0px;\"" : "style=\"display: none\"" %>>
			<div id="amazonImg" class="border" style="width: 75px; height: 75px; background: white;">
				<img src="images/no_img.gif" width="75" height="75" alt="No Image" />
			</div>
			<br />
			<div id="amazonCheck">
				<a href="#" onclick="CheckAmazonLink(false);">Check for<br />album cover</a>
			</div>
			<div id="amazonExplicitCheck" style="display: none">
				<a href="#" onclick="CheckAmazonLink(true);">Re-check for<br />album cover</a>
			</div>
			<div id="amazonWorkingArea" style="display: none">
				<em>Working...</em>
			</div>
		</td>
	</tr>
	<tr id="amazonArtistsRow" style="display: none;">
		<td colspan="3">
			<div id="amazonArtists" style="width: 240px; font-size: x-small; overflow: auto; display: none;"></div>
		</td>
	</tr>
	<tr>
		<td><strong>Lyricist</strong></td>
		<td>&nbsp;</td>
		<td><asp:TextBox ID="Lyricist" runat="server" Width="175"></asp:TextBox></td>
	</tr>
	<tr>
		<td><strong>Song</strong></td>
		<td>&nbsp;</td>
		<td><asp:TextBox ID="Song" runat="server" Width="175"></asp:TextBox></td>
	</tr>
	<tr id="amazonTracksRow" style="display: none;">
		<td colspan="3">
			<div id="amazonTracks" style="width: 240px; font-size: x-small; overflow: auto; display: none;"></div>
		</td>
	</tr>
	<tr>
		<td><strong>Album</strong></td>
		<td>&nbsp;</td>
		<td><asp:TextBox ID="Album" runat="server" name="Album" Width="175"></asp:TextBox></td>
	</tr>
	<tr id="amazonAlbumsRow" style="display: none;">
		<td colspan="3">
			<div id="amazonAlbums" style="width: 240px; font-size: x-small; overflow: auto; display: none;"></div>
		</td>
	</tr>
	<tr>
		<td><strong>Genre</strong></td>
		<td>&nbsp;</td>
		<td><asp:DropDownList ID="Genre" runat="server" />
		</td>
	</tr>
	<tr>
		<td valign="top"><strong>Quote</strong></td>
		<td>&nbsp;</td>
		<td colspan="2"><asp:TextBox ID="Quote" runat="server" TextMode="MultiLine" Width="300" Height="200"></asp:TextBox></td>
	</tr>
	<tr>
		<td colspan="4"><a href="#" <%= (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) ? "onclick=\"SetMultiTaskType('submit'); document.forms[0].submit(); return false;\"" : "onclick=\"alert('You must be logged in to submit a quote.'); return false;\"" %>>Add quote</a></td>
	</tr>
</table>
</asp:Content>

<asp:Content ContentPlaceHolderID="NotAuthorized" ID="NotAuthorized" runat="server">
	Nun-uh! I am pretty sure you need to be logged in to do this...
</asp:Content>