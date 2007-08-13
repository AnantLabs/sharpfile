<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Archives.ascx.cs" Inherits="Controls_Blog_Archives" %>

<div id="divLeftArchive" runat="server">
	<img src="../images/down_arrow.png" alt="" id="leftArchiveArrow" />
	<div id="leftArchiveTitle">
		<asp:Label ID="lblLeftTitle" runat="server" />
	</div>
</div>
<div id="divRightArchive" runat="server">
	<img src="../images/down_arrow.png" alt="" id="rightArchiveArrow" />
	<div id="rightArchiveTitle">
		<asp:Label ID="lblRightTitle" runat="server" />
	</div>	
</div>

<asp:Repeater ID="rptArchives" runat="server">
	<HeaderTemplate>
		<ul>
	</HeaderTemplate>
	<ItemTemplate>
		<li><a href='Archive.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>'><%# DataBinder.Eval(Container.DataItem, "StartDate") %> - <%# DataBinder.Eval(Container.DataItem, "EndDate") %></a> (<%# DataBinder.Eval(Container.DataItem, "Count") %>)</li>
	</ItemTemplate>
	<FooterTemplate>
		</ul>
	</FooterTemplate>
</asp:Repeater>

<asp:Label ID="lblNoArchives" runat="server" Visible="false" Text="No archive, yet." />