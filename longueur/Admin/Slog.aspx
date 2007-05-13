<%@ Page Language="C#" MasterPageFile="~/Admin/admin.master" AutoEventWireup="true" CodeFile="Slog.aspx.cs" Inherits="Admin_Slog" Title="Untitled Page" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	<strong>Title</strong><br />
	<asp:TextBox ID="txtTitle" runat="server" />
	<br />
	
	<strong>Content</strong><br />
	<asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Width="400" Height="400" />
	<br />
	
	<asp:Button ID="btnSubmit" runat="server" Text="Submit" />
</asp:Content>

