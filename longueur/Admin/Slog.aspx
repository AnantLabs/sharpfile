<%@ Page Language="C#" MasterPageFile="~/Admin/admin.master" AutoEventWireup="true" CodeFile="Slog.aspx.cs" Inherits="Admin_Slog" Title="Untitled Page" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	Title: <asp:TextBox ID="txtTitle" runat="server" />
	<br />
	
	Content: <asp:TextBox ID="txtContent" runat="server" />
	<br />
	
	<asp:Button ID="btnSubmit" runat="server" Text="Submit" />
</asp:Content>

