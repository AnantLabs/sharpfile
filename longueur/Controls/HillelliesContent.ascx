<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HillelliesContent.ascx.cs" Inherits="Controls_HillelliesEntry" %>

<div id="title">
	<div id="titleName">
		<asp:Label ID="lblTitle" runat="server" />
	</div>
	<div id="titleImage">
		<asp:Image ID="imgTitle" runat="server" />
	</div>
</div>
<br />

<asp:Repeater ID="rptContent" runat="server">
	<ItemTemplate>
		<div id="tags" style="display: none">
			Tags are going to go here.
		</div>
		
		<div class="entryInformation">
			<div class="entryTag">
				<img src="blob.jpg" alt="" />
			</div>
			<div class="entryDescription">
				<div class="title">
					<%# DataBinder.Eval(Container.DataItem, "Title").ToString() %>
				</div>
				<div class="dateTime">
					<%# DataBinder.Eval(Container.DataItem, "DateTime").ToString() %>
				</div>
				<br />
			</div>
		</div>
		
		<div class="content">
			<%# DataBinder.Eval(Container.DataItem, "Content").ToString() %>
		</div>
		<br />
		<br />
	</ItemTemplate>
</asp:Repeater>