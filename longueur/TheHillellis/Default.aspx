<%@ Page Language="C#" MasterPageFile="~/TheHillellis/TheHillellis.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="TheHillellis_Default" %>
<%@ Register TagPrefix="lng" TagName="Content" Src="~/Controls/HillelliesContent.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	<div id="leftContent">
		<lng:Content UserName="lynn" Title="Lynn" ImageUrl="blob.jpg" ID="ctlLeftContent" runat="server" />
	</div>
	<div id="rightContent">
		<lng:Content UserName="adam" Title="Adam" ImageUrl="~/TheHillellies/images/lost_river.jpg" ID="ctlRightContent" runat="server" />
	</div>
	<div id="archives">
		<h1>Archives</h1>
	</div>
</asp:Content>

