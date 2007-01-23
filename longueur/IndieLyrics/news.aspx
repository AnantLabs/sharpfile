<%@ Page Language="C#" MasterPageFile="~/IndieLyrics/template.master" AutoEventWireup="true" CodeFile="news.aspx.cs" Inherits="news" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">

<asp:Repeater ID="NewsItems" Runat="server">
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
				<p><span class="date"><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DateStamp")).ToString("MM.dd.yyyy") %></span><%# DataBinder.Eval(Container.DataItem, "NewsText") %></p><br />
		</ItemTemplate>
		<FooterTemplate>
		</FooterTemplate>
	</asp:Repeater>
	
</asp:Content>

