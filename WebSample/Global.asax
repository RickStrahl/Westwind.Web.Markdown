<%@ Application Language="C#" %>
<%@ Import Namespace="Markdig" %>
<%@ Import Namespace="Markdig.Extensions.AutoIdentifiers" %>
<%@ Import Namespace="Westwind.Web.Markdown" %>
<%@ Import Namespace="Westwind.Web.Markdown.MarkdownParser" %>


<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup

        // OPTIONAL - override markdown parser pipeline with specific options
        // * use to minimize features you need specifically
        // * customize or add additional features
        // * default has most options enabled
        MarkdownParserMarkdig.OnCreateMarkdigPipeline = (usePragmaLines) =>
        {
            var builder = new Markdig.MarkdownPipelineBuilder()
                .UseEmphasisExtras()                
                .UsePipeTables()
                .UseGridTables()
                .UseAutoLinks() // URLs are parsed into anchors
                .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)                
                .UseYamlFrontMatter()
                .UseEmojiAndSmiley(true);

            if (usePragmaLines)
                builder = builder.UsePragmaLines();

            return builder;
        };

        MarkdownHttpHandler.Configuration.SanitizeHtml = true;
        MarkdownHttpHandler.Configuration.MarkdownTemplatePagePath = "~/_MarkdownPage.aspx";

    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

</script>
