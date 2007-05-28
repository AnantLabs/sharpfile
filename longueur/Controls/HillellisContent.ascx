<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HillellisContent.ascx.cs" Inherits="Controls_HillellisEntry" %>

<asp:PlaceHolder ID="phTopHat" runat="server">
	<div id="titleImage">
		<asp:Image ID="imgTitle" runat="server" />
	</div>
	<br />

	<div id="tags" style="display: none">
		Tags are going to go here.
	</div>
	<br />
</asp:PlaceHolder>

<asp:Repeater ID="rptContent" runat="server">
	<ItemTemplate>
		<div class="entry">
			<div class="entryInformation">
				<div class="entryTag" style="display: none">
					<img src="../TheHillellis/images/cupcake_t.png" alt="" />
				</div>
				<div class="entryDescription">
					<h2><%# DataBinder.Eval(Container.DataItem, "Title") %></h2>
					<div class="dateTime">
						<%# DataBinder.Eval(Container.DataItem, "DateTime").ToString() %>
					</div>
					<br />
				</div>
			</div>
			
			<div class="content">
				<%# DataBinder.Eval(Container.DataItem, "Content").ToString() %>
			</div>
			<div class="permalink">
				<a href='../TheHillellis/Permalink.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Permalink</a>
			</div>
			<div style='<%# (int.Parse(DataBinder.Eval(Container.DataItem, "Id").ToString()) == getLastId()) ? "display: none" : "" %>'>
				<br /><br />
			</div>
		</div>
	</ItemTemplate>
</asp:Repeater>

<asp:Label ID="lblMessage" runat="server" Visible="false" />