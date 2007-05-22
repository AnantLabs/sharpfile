<%@ Page Language="C#" MasterPageFile="~/TheHillellis/TheHillellis.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="TheHillellis_Default" %>
<%@ Register TagPrefix="lng" TagName="Content" Src="~/Controls/HillellisContent.ascx" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	<div id="leftContent">
		<lng:Content ID="ctlLeftContent" runat="server" />
	</div>
	<div id="middleSeperator">
		&nbsp;
	</div>
	<div id="rightContent">
		<lng:Content ID="ctlRightContent" runat="server" />
	</div>
</asp:Content>