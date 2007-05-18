<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HillelliesContent.ascx.cs" Inherits="Controls_HillelliesEntry" %>
<asp:Label ID="lblTitle" runat="server" />
<asp:Image ID="imgTitle" runat="server" />
<br />

<asp:Repeater ID="rptContent" runat="server">
	<ItemTemplate>
		<div class="title">
			<%# DataBinder.Eval(Container.DataItem, "Title").ToString() %>
		</div>
		<div class="date">
			<%# DataBinder.Eval(Container.DataItem, "DateTime").ToString() %>
		</div>
		<br />
		
		<div class="user">
			<%# DataBinder.Eval(Container.DataItem, "Name").ToString() %>
		</div>
		<br />
		
		<div class="content">
			<%# DataBinder.Eval(Container.DataItem, "Content").ToString() %>
		</div>
		<br />
	</ItemTemplate>
</asp:Repeater>

Some content