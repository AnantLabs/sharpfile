<%@ Page Language="C#" MasterPageFile="~/IndieLyrics/template.master" AutoEventWireup="true" CodeFile="quote.aspx.cs" Inherits="quote" Title="IndieLyrics: Quote" ValidateRequest="false" %>
<%@ Register TagPrefix="ajax" Namespace="MagicAjax.UI.Controls" Assembly="MagicAjax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">

<script language="JavaScript" type="text/javascript">
	function SetMultiTaskType(action) {
		this.document.forms[0].MultiTaskType.value = action;
	}
	
	function ToggleCommentArea() {
		if (this.document.getElementById('showLinkArea').style.display == 'none') {
			this.document.getElementById('showLinkArea').style.display = '';
			this.document.getElementById('commentArea').style.display = 'none';
		} else {
			this.document.getElementById('showLinkArea').style.display = 'none';
			this.document.getElementById('commentArea').style.display = '';
		}
	}
	
	function ToggleCommentTextArea(commentId) {
		if (this.document.getElementById('editCommentTextArea_' + commentId).style.display == 'none') {
			this.document.getElementById('editCommentTextArea_' + commentId).style.display = '';
			this.document.getElementById('commentTextArea_' + commentId).style.display = 'none';
		} else {
			this.document.getElementById('editCommentTextArea_' + commentId).style.display = 'none';
			this.document.getElementById('commentTextArea_' + commentId).style.display = '';
		}
	}
	
	function RefreshCommentTextArea(commentId) {
		this.document.getElementById('refreshCommentTextArea_' + commentId).style.display = '';
		this.document.getElementById('editCommentTextArea_' + commentId).style.display = 'none';
		this.document.getElementById('commentTextArea_' + commentId).style.display = 'none';
		this.document.getElementById('actionArea_' + commentId).style.display = 'none';
	}
	
	function DeleteCommentTextArea(commentId) {
		this.document.getElementById('deleteCommentTextArea_' + commentId).style.display = '';
		this.document.getElementById('editCommentTextArea_' + commentId).style.display = 'none';
		this.document.getElementById('commentTextArea_' + commentId).style.display = 'none';
		this.document.getElementById('actionArea_' + commentId).style.display = 'none';
	}
	
	function SubmitCommentTextArea() { 
		this.document.getElementById('commentTextArea').style.display = 'none'; 
		this.document.getElementById('refreshCommentTextArea').style.display = '';
	}
</script>

<ajax:AjaxPanel ID="ajaxPanel" runat="server" AjaxCallConnection="asynchronous">
	<asp:Repeater ID="quoteDetails" runat="server">
		<ItemTemplate>
			<table>
				<tr>
					<td><strong>Added by</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><a href='user.aspx?ID=<%# DataBinder.Eval(Container.DataItem, "UserID").ToString() %>'><%# DataBinder.Eval(Container.DataItem, "UserName").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : DataBinder.Eval(Container.DataItem, "UserName").ToString() %></a></td>
				</tr>
				<tr>
					<td><strong>Artist Name</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><%# DataBinder.Eval(Container.DataItem, "ArtistName").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : "<a href='artist.aspx?ID=-1'>" + DataBinder.Eval(Container.DataItem, "ArtistName").ToString() + "</a>" %></td>
				</tr>
				<tr>
					<td><strong>Lyricist</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><%# DataBinder.Eval(Container.DataItem, "LyricistName").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : "<a href='artist.aspx?ID=-1'>" + DataBinder.Eval(Container.DataItem, "LyricistName").ToString() + "</a>" %></td>
				</tr>
				<tr>
					<td><strong>Song Name</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><%# DataBinder.Eval(Container.DataItem, "SongName").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : "<a href='song.aspx?ID=-1'>" + DataBinder.Eval(Container.DataItem, "SongName").ToString() + "</a>" %></td>
				</tr>
				<tr>
					<td><strong>Album</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><%# DataBinder.Eval(Container.DataItem, "AlbumName").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : "<a href='album.aspx?ID=-1'>" + DataBinder.Eval(Container.DataItem, "AlbumName").ToString() + "</a>"%></td>
				</tr>
				<tr>
					<td><strong>Genre</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><%# DataBinder.Eval(Container.DataItem, "GenreName").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : DataBinder.Eval(Container.DataItem, "GenreName").ToString() %></td>
				</tr>
				<tr>
					<td><strong>Average Rating</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><%# DataBinder.Eval(Container.DataItem, "AvgRating").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : Math.Round(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "AvgRating").ToString()), 2).ToString() %></td>
				</tr>
				<tr style='<%# Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated ? "" : "DISPLAY: none" %>'>
					<td><strong>Rate</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><asp:DropDownList ID="rateQuote" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rateQuote_Event">
						<asp:ListItem Text="" Value=""></asp:ListItem>
						<asp:ListItem Text="0" Value="0"></asp:ListItem>
						<asp:ListItem Text="1" Value="1"></asp:ListItem>
						<asp:ListItem Text="2" Value="2"></asp:ListItem>
						<asp:ListItem Text="3" Value="3"></asp:ListItem>
						<asp:ListItem Text="4" Value="4"></asp:ListItem>
						<asp:ListItem Text="5" Value="5"></asp:ListItem>
					</asp:DropDownList></td>
				</tr>
				<tr>
					<td colspan="3">&nbsp;</td>
				</tr>
				<tr>
					<td valign="top"><strong>Quote</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><%# DataBinder.Eval(Container.DataItem, "QuoteText").ToString() == string.Empty ? "<span class=greyout>n/a</span>" : DataBinder.Eval(Container.DataItem, "QuoteText").ToString() %></td>
				</tr>
			</table>
		</ItemTemplate>
	</asp:Repeater>
	<p>&nbsp;</p>
  
	<div id="showLinkArea" style="DISPLAY: none">
	<table>
		<tr>
			<td><a href="" onclick="ToggleCommentArea();return false;"><img src="~/IndieLyrics/Images/plus.gif" title="Show Comments" runat="server" id="Img2" style="border: none;" alt="Show Comments" /></a></td>
			<td><h3><a href="" onclick="ToggleCommentArea();return false;">Comments</a></h3></td>
		</tr>
	</table>
	</div>
	
	<p>&nbsp;</p>
  
	<div id="commentArea">
	  <asp:Repeater ID="quoteComments" Runat="server" OnItemCommand="repeater_Event">
	  <HeaderTemplate>
		<table>
			<tr>
				<td style="vertical-align: middle"><a href="" onclick="ToggleCommentArea();return false;"><img src="~/IndieLyrics/Images/minus.gif" alt="Hide Comments" title="Hide Comments" runat="server" id="Img1" style="border: none;" /></a></td>
				<td style="vertical-align: middle"><a href="" onclick="ToggleCommentArea();return false;">Comments</a></td>
			</tr>
		</table>
	  </HeaderTemplate>
	  <ItemTemplate>
			<div id="comments">
				<input type="hidden" id="commentId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' />
				<span class="actions" id='actionArea_<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'><a href="#" onclick="ToggleCommentTextArea('<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'); return false;"><img style='<%# Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated && (((SiteUser)Session[Constants.CurrentUser]).Id == int.Parse(DataBinder.Eval(Container.DataItem, "UserID").ToString()) || ((SiteUser)Session[Constants.CurrentUser]).UserType == UserType.Admin) ? "border: none" : "border: none; DISPLAY: none" %>' src='<%# Request.ApplicationPath %>/images/comment_edit.png' alt="Edit Comment" title="Edit Comment" /></a>&nbsp;<asp:ImageButton ID="deleteBtn" CommandName="delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' Visible='<%# Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated && (((SiteUser)Session[Constants.CurrentUser]).Id == int.Parse(DataBinder.Eval(Container.DataItem, "UserID").ToString()) || ((SiteUser)Session[Constants.CurrentUser]).UserType == UserType.Admin) ? true : false %>' ImageUrl="~/IndieLyrics/Images/comment_delete.png" AlternateText="Delete Comment" ToolTip="Delete Comment" runat="server" /></span>
				<span class="user"><%# DataBinder.Eval(Container.DataItem, "UserName") %></span>
				<p>&nbsp;</p>
				<span class="text">
					<div id='commentTextArea_<%# DataBinder.Eval(Container.DataItem, "CommentID") %>'><%# DataBinder.Eval(Container.DataItem, "CommentText") %></div>
					<div id='editCommentTextArea_<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' style="DISPLAY: none">
						<textarea id="editedCommentText" cols="48" rows="3" runat="server" style='<%# SiteUser.IsCurrentUserAuthorized() ? "" : "DISPLAY: none;" %>'><%# DataBinder.Eval(Container.DataItem, "CommentText") %></textarea>
						<div class="actions"><asp:ImageButton ID="saveBtn" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' CommandName="save" ImageUrl="~/IndieLyrics/Images/comment_save.png" AlternateText="Save Comment" ToolTip="Save Comment" runat="server" Visible='<%# SiteUser.IsCurrentUserAuthorized() %>' /></div>
						<p>&nbsp;</p>
					</div>
					<div id='refreshCommentTextArea_<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' style="DISPLAY: none">
						<em>Refreshing comment...</em>
					</div>
					<div id='deleteCommentTextArea_<%# DataBinder.Eval(Container.DataItem, "CommentID") %>' style="DISPLAY: none">
						<em>Deleting comment...</em>
					</div>
					<p>&nbsp;</p>
				</span>
				<span class="date"><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DateTimeStamp")).ToString("MM.dd.yyyy") %></span>
				<p>&nbsp;</p>
				<span style='<%# DataBinder.Eval(Container.DataItem, "LastEditedDateTimeStamp") == Convert.DBNull ? "DISPLAY: none; float: right" : "float: right" %>'><em>Last Edited: <%# DataBinder.Eval(Container.DataItem, "LastEditedDateTimeStamp") == Convert.DBNull ? "n/a" : ((DateTime)DataBinder.Eval(Container.DataItem, "LastEditedDateTimeStamp")).ToString("MM.dd.yyyy")%></em></span>
				<p style='<%# DataBinder.Eval(Container.DataItem, "LastEditedDateTimeStamp") == Convert.DBNull ? "DISPLAY: none;" : "" %>'>&nbsp;</p>
			</div>
	  </ItemTemplate>
	  </asp:Repeater>
	  
	  <div class="add_comment" id="AddComment" runat="server">
	        <br />
			<h3>Add a Comment</h3>
			
			<div class="warning">
			    <asp:Label ID="ErrorMessage" runat="server"></asp:Label>
			</div>
			
			<div id="commentTextArea">
			<table>
				<tr>
					<td><strong>User</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><asp:Label ID="CommentUser" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td valign="top"><strong>Comment</strong></td>
					<td>&nbsp;&nbsp;</td>
					<td><asp:TextBox ID="CommentText" TextMode="MultiLine" Runat="server" Height="60" Width="300" CausesValidation="false" Text=""></asp:TextBox></td>
				</tr>
				<tr>
					<td colspan="3" style="font-style: italic">Note that the only available html tags are: <br />
					&lt;br&gt;, &lt;br /&gt;, &lt;em&gt;, &lt;/em&gt;, &lt;strong&gt; and &lt;/strong&gt;</td>
				</tr>
				<tr>
					<td colspan="3"><asp:LinkButton OnClick="submitBtn_Event" ID="submitBtn" runat="server" Text="Submit Comment"></asp:LinkButton></td>
				</tr>
			</table>
			</div>
			<div id="refreshCommentTextArea" style="display: none;">
				<em>Submitting comment...</em>
			</div>
		</div>
	</div>
</ajax:AjaxPanel>

</asp:Content>

