<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master"  %>
<%@ Import Namespace="Westwind.Web.Markdown" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <%
        var model = Context.Items[MarkdownHttpHandler.ModelKey] as MarkdownModel;
    %>
    <div class="container">                
        <%= model.RenderedMarkdown %>
    </div>
</asp:Content>    
