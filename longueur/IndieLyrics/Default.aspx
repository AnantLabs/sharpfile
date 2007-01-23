<%@ Page MasterPageFile="~/IndieLyrics/Template.master" AutoEventWireup="true" Language="c#" CodeFile="Default.aspx.cs" Inherits="_default" Title="IndieLyrics: Home" %>
<%@ Register TagPrefix="lng" TagName="NewestQuotes" Src="~/IndieLyrics/Controls/NewestQuotes.ascx" %>
<%@ Register TagPrefix="lng" TagName="HighestRatedQuotes" Src="~/IndieLyrics/Controls/HighestRatedQuotes.ascx" %>
<%@ Register TagPrefix="lng" TagName="MostArtistQuotes" Src="~/IndieLyrics/Controls/MostArtistQuotes.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
	<asp:Literal ID="JsMsg" Runat="server" />
	
	<div class="greyBorder">
		Add, find, and vote on lyrics which transcend the pretentious or arbitrary, to achieve genuine insight... this website documents what I (and you, theoretically) consider the best indie lyrics out there.
	</div>
	
	<table>
		<tr>
			<td class="greyBorder" style="width: 33%; vertical-align: top"><lng:NewestQuotes id="NewestQuotes" runat="server" /></td>
			<td class="greyBorder" style="width: 33%; vertical-align: top; background-color: #eeeeee"><lng:HighestRatedQuotes ID="HighestRatedQuotes" runat="server" /></td>
			<td class="greyBorder" style="width: 33%; vertical-align: top"><lng:MostArtistQuotes ID="MostArtistQuotes" runat="server" /></td>
		</tr>
	</table>
</asp:Content>
