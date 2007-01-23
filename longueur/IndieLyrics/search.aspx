<%@ Page Language="C#" MasterPageFile="~/IndieLyrics/template.master" AutoEventWireup="true" CodeFile="search.aspx.cs" Inherits="search" Title="IndieLyrics: Search" %>
<%@ Register TagPrefix="ajax" Namespace="MagicAjax.UI.Controls" Assembly="MagicAjax" %>
<%@ Import Namespace="Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">

<ajax:AjaxPanel ID="ajaxPanel" runat="server" AjaxCallConnection="asynchronous">
	<div class="greyBorderCentered">
		<asp:Repeater ID="rptAlphaNumerals" runat="server">
			<HeaderTemplate>
				<input type="hidden" value='<%# SearchString %>' name="SearchStringHiddenForm" />
				<div class="">
			</HeaderTemplate>
			<ItemTemplate>
					<asp:LinkButton ID="lnkSearch" runat="server" OnClick="PopulateResults" Text='<%# Container.DataItem %>' CommandArgument='<%# Container.DataItem %>' Visible='<%# ((string)Container.DataItem).Equals(SearchString) ? false : true %>' />
					<%# ((string)Container.DataItem).Equals(SearchString) ? "<strong>" + (string)Container.DataItem + "</strong>" : "" %>
					<%# !IsLastAlphaNumeralItem(Container) ? "| " : "" %>
			</ItemTemplate>
			<FooterTemplate>
				</div>
				<br />
			</FooterTemplate>
		</asp:Repeater>
    
		<table>
		<tr>
			<td>Search by</td>
			<td colspan="2"><asp:DropDownList ID="ddlSearchType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PopulateResults" /></td>
		</tr>
		<tr style="display: none">
			<td style="border: solid 1px black">Return</td>
			<td><asp:TextBox ID="txtMaxResults" runat="server" Text="10" Width="30" /></td>
			<td>Quotes</td>
		</tr>
		</table>
    </div>
    <br />
    
    <asp:Repeater ID="rptList" runat="server">
		<HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
				<%# GetDetails(Container) %>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href='<%# "quote.aspx?id=" + DataBinder.Eval(Container.DataItem, "QuoteId") %>'><%# Common.General.GetSubString(DataBinder.Eval(Container.DataItem, "QuoteText").ToString(), 50) %></a><br>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
    
</ajax:AjaxPanel>

<br />
<br />
<br />

</asp:Content>

