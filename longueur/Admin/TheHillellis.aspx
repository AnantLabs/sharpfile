<%@ Page Language="C#" MasterPageFile="~/Admin/admin.master" AutoEventWireup="true" CodeFile="TheHillellis.aspx.cs" Inherits="Admin_TheHillellis" Title="Admin: The Hillellis" ValidateRequest="false" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	<asp:Repeater ID="rptSlogs" runat="server" Visible="true">
		<HeaderTemplate>
			<table>
			<tr>
				<td style="font-weight: bold">Id</td>
				<td style="font-weight: bold">Name</td>
				<td style="font-weight: bold">Title</td>
				<td style="font-weight: bold">DateTime</td>
				<td style="font-weight: bold">Actions</td>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td><%# DataBinder.Eval(Container.DataItem, "Id").ToString() %></td>
				<td><%# DataBinder.Eval(Container.DataItem, "Name").ToString() %></td>
				<td><%# DataBinder.Eval(Container.DataItem, "Title").ToString() %></td>		
				<td><%# DataBinder.Eval(Container.DataItem, "DateTime").ToString() %></td>
				<td><a href='TheHillellis.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id").ToString() %>'>Edit</a></td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:Repeater>

	<div id="divNewEntry" runat="server">
		<br />
		<br />
		<strong>Add new entry...</strong><br />
		
		<strong>Title</strong><br />
		<asp:TextBox ID="txtNewTitle" runat="server" />
		<br />
		
		<strong>Content</strong><br />
		<asp:TextBox ID="txtNewContent" runat="server" TextMode="MultiLine" Width="80%" Height="600" />
		<br />
		
		<asp:Button ID="btnNewSubmit" runat="server" Text="Submit" />
	</div>
	
	<div id="divSlogInfo" runat="server" visible="false">
		<a href="TheHillellis.aspx">...Back</a><br />
		<br />
		<br />

		<strong>Id</strong><br />
		<asp:Label ID="lblId" runat="server" />
		<br />
		
		<strong>Name</strong><br />
		<asp:TextBox ID="txtName" runat="server" />
		<br />
		
		<strong>Title</strong><br />
		<asp:TextBox ID="txtTitle" runat="server" />
		<br />
		
		<strong>Content</strong><br />
		<asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Width="80%" Height="600" />
		<br />
		
		<strong>DateTime</strong>
		<asp:Label ID="lblDateTime" runat="server" />
		<br />
		
		<asp:Button ID="btnDelete" runat="server" Text="Delete" />
		<asp:Button ID="btnSave" runat="server" Text="Save" />
	</div>
	
	<asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="warning" />
</asp:Content>

