<%@ Page Language="C#" MasterPageFile="~/Admin/admin.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Admin_Users" Title="Admin: Users" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" EnableViewState="false">
	<asp:Repeater ID="rptUsers" runat="server" Visible="true">
		<HeaderTemplate>
			<table>
			<tr>
				<td style="font-weight: bold">Id</td>
				<td style="font-weight: bold">Username</td>
				<td style="font-weight: bold">Email</td>
				<td style="font-weight: bold">UserType</td>
				<td style="font-weight: bold">DateTime</td>
				<td style="font-weight: bold">Edit</td>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td><%# DataBinder.Eval(Container.DataItem, "Id").ToString() %></td>
				<td><%# DataBinder.Eval(Container.DataItem, "Name").ToString() %></td>
				<td><%# DataBinder.Eval(Container.DataItem, "Email").ToString() %></td>		
				<td><%# DataBinder.Eval(Container.DataItem, "UserType").ToString() %></td>
				<td><%# DataBinder.Eval(Container.DataItem, "DateTime").ToString() %></td>
				<td><a href='users.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id").ToString() %>'>Edit</a></td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:Repeater>
			
	<div id="divNewUser" runat="server" visible="true">
		<br />
		<br />
		<strong>Add a new user...</strong>
		<br />
		
		<strong>Name</strong><br />
		<asp:TextBox ID="txtNewName" runat="server" />
		<br />
		
		<strong>Email</strong><br />
		<asp:TextBox ID="txtNewEmail" runat="server" />
		<br />
		
		<strong>Password</strong><br />
		<asp:TextBox ID="txtNewPassword" runat="server" />
		<br />
		
		<strong>Type</strong><br />
		<asp:DropDownList ID="ddlNewUserType" runat="server" />
		<br />
		
		<asp:Button ID="btnNewSave" runat="server" Text="Save" />
	</div>

	<div id="divUserInfo" runat="server" visible="false">
		<a href="Users.aspx">...Back</a><br />
		<strong>Be careful doing this, it may affect wierd things. In fact, you probably shouldn't be here.</strong>
		<br />
		<br />

		<strong>Id</strong><br />
		<asp:Label ID="lblId" runat="server" />
		<br />
		
		<strong>Name</strong><br />
		<asp:TextBox ID="txtName" runat="server" />
		<br />
		
		<strong>Email</strong><br />
		<asp:TextBox ID="txtEmail" runat="server" />
		<br />
		
		<strong>Password</strong><br />
		<asp:TextBox ID="txtPassword" runat="server" />
		<br />
		
		<strong>Type</strong><br />
		<asp:DropDownList ID="ddlUserType" runat="server" />
		<br />
		
		<strong>DateTime</strong>
		<asp:Label ID="lblDateTime" runat="server" />
		<br />
		
		<asp:Button ID="btnSave" runat="server" Text="Save" />
	</div>
	
	<asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="warning" />
</asp:Content>