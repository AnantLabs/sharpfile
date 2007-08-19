<%@ Page Language="C#" MasterPageFile="~/Admin/admin.master" AutoEventWireup="true" CodeFile="TheHillellis.aspx.cs" Inherits="Admin_TheHillellis" Title="Admin: The Hillellis" ValidateRequest="false" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
<script type="text/javascript" language="javascript">
    function addTag(tagId, newTagId, tag) {
        var txtTags = document.getElementById(tagId);
        var txtNewTags = document.getElementById(newTagId);
        
        if (txtTags != null) {
            if (txtTags.value.indexOf(tag) < 0) {  
                if (txtTags.value != '') {
                    tag = ' ' + tag;
                }
                
                txtTags.value += tag;
            } else {
                alert('You are trying to add the same tag, ' + tag + ', twice. That doesn\'t make much sense.');
            }            
        } else if (txtNewTags != null) {
            if (txtNewTags.value.indexOf(tag) < 0) {  
                if (txtNewTags.value != '') {
                    tag = ' ' + tag;
                }
            
                txtNewTags.value += tag;
            } else {
                alert('You are trying to add the same tag, ' + tag + ', twice. That doesn\'t make much sense.');
            }
        }
    }
</script>

	<asp:Repeater ID="rptSlogs" runat="server" Visible="true">
		<HeaderTemplate>
		    <strong>Entries:</strong>
		    <div style="overflow: auto; overflow-y: auto; overflow-x: hidden; height: 200px; border: solid 1px black; margin: 5px; padding: 5px">
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
				<td><%# DataBinder.Eval(Container.DataItem, "Id") %></td>
				<td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
				<td><%# DataBinder.Eval(Container.DataItem, "Title") %></td>		
				<td><%# DataBinder.Eval(Container.DataItem, "DateTime") %></td>
				<td><a href='TheHillellis.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Edit</a></td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
			</div>
		</FooterTemplate>
	</asp:Repeater>
	
	<asp:Repeater ID="rptTags" runat="server">
	    <HeaderTemplate><strong>Available Tags:</strong>
	    </HeaderTemplate>
	    <ItemTemplate>
	        <a href="javascript:addTag('<%# txtTags.ClientID %>','<%# txtNewTags.ClientID %>','<%# DataBinder.Eval(Container.DataItem, "Name") %>')"><%# DataBinder.Eval(Container.DataItem, "Name") %></a>
	    </ItemTemplate>
	</asp:Repeater>

	<div id="divNewEntry" runat="server">
		<br />
		<br />
		<strong>Add new entry...</strong><br />
		
		<strong>Title</strong><br />
		<asp:TextBox ID="txtNewTitle" runat="server" MaxLength="256" Width="80%" />
		<br />
		
		<strong>Tags</strong> (seperate different tags with a space)<br />
		<asp:TextBox ID="txtNewTags" runat="server" MaxLength="500" Width="80%" />
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
		
		<strong>Tags</strong> (tags are seperated with a space)<br />
		<asp:TextBox ID="txtTags" runat="server" MaxLength="500" Width="80%" />
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

