---
title: A Literal Markdown Control for ASP.NET WebForms
abstract: Spent some time last night creating a small ASP.NET Server control that can render literal Markdown text inside of ASPX pages and turn the literal text into Markdown. It's a very simple control, but it makes it lot easier to edit documents that contain simple formatted text content without having to deal with angle brackets for lengthier text.
keywords: Markdown, ASP.NET, WebForms, Literal, Editing
categories: ASP.NET Markdown
weblogName: West Wind Web Log
postId: 397177
postDate: 2018-09-08T16:04:22.8116253-07:00
---
# A Literal Markdown Control for ASP.NET WebForms

![](WritingFeathers.jpg)

It's been a while since I've used WebForms in an application directly, but I have a ton of mostly static content (with some minor code additions) that still lives in ASPX pages. A lot of main site content pages that do a few simple things like checking for versions of software, displaying version numbers etc.

As regulars here know I've been working with **Markdown** a lot lately, especially related to [Markdown Monster](https://markdownmonster.west-wind.com) and I'm always looking for new ways to get Markdown into the content I put out. Last night I was editing a couple product pages and realized I really should not be writing my text with angle brackets, but use Markdown instead. 

> Ironically when writing plain text in HTML documents I almost naturally fall into wanting to write Markdown - I find myself writing Markdown syntax in HTML and wondering why it's not working :-)

IAC, I often write fairly plain text in content pages that doesn't require any fancy HTML formatting and writing and even reading  that text coherently with angle brackets is a pain. So - the obvious thing to do here is to embed the plain or simple formatted text into the document **as Markdown** and let that markdown be parsed at runtime (or generation time in some cases). Or - if the text is really lengthy - I actually edit the text in [Markdown Monster](https://markdownmonster.west-wind.com) (or you can use whatever other Editor your prefer) and then paste the result into the document to be rendered - because it's much easier to see what the output looks like in an editor with a live preview, spell checking etc.

##AD##

## Westind.Web.Markdown
This isn't a new idea of course. If you use a CMS you likely already have this built-in and there are ASP.NET MVC and ASP.NET Core Html and Tag Helpers that provide literal translation from Markdown to HTML. But looking around last night I didn't see a WebForms implementation, so I quickly whipped up a simple WebForms server control.

> #### @icon-info-circle What about ASP.NET Core?
> This post discusses System.Web MVC and WebForms Markdown support. If you're using ASP.NET Core, that a look at:
>
> * [A Markdown TagHelper and Markdown Parser for ASP.NET Core](https://weblog.west-wind.com/posts/2018/Mar/23/Creating-an-ASPNET-Core-Markdown-TagHelper-and-Parser)
>
> * [Creating a generic Markdown Page Handler with ASP.NET Core Middleware](https://weblog.west-wind.com/posts/2018/Apr/18/Creating-a-generic-Markdown-Page-Handler-using-ASPNET-Core-Middleware)

The control lets me write the following inline Markdown:

```html
<div class="container">
    <ww:Markdown runat="server" id="md1">
        ### 1.6.2
        *September 10th, 2017*
        
        * **Keyboard support for Context Menu**  
        You can now pop up the context menu via keyboard using the Windows context menu key (or equivalent). The menu is now cursor navigable. This brings spell checking and various edit operations to keyboard only use.
        
        * **Fix: `UseSingleWindow=false` no longer opens Remembered Documents**   
        When not running in `UseSingleWindow` mode, the `RememberLastDocumentsLength` setting has no effect and no previous windows are re-opened. This is so multiple open windows won't open the same documents all the time. In `UseSingleWindow` mode last documents are remembered and opened when starting MM for the first time.
    </ww:Markdown>
</div>
```

There's also simple static Markdown parsing support (courtesy of the awesome [Markdig Markdown Parser](https://github.com/lunet-io/markdig)):

```html
<%= Markdown.Parse(Model.MarkdownNotesText, sanitizeHtml: true) %>
```

```cs
@Markdown.ParseHtml(Model.MarkdownNotesText, sanitizeHtml: true) %>
```

Anyway, you can find the code and a Nuget package here:

* [Westwind.Web.Markdown on GitHub](https://github.com/RickStrahl/Westwind.Web.Markdown)
* [Westwind.Web.Markdown on Nuget](https://www.nuget.org/packages/Westwind.Web.Markdown)

There's a lot more information on how to use the control and a few of the options available.

## Creating a Markdown Literal Text Control
It's been a long time since I've built an ASP.NET Server control and I had to refresh my memory. I haven't done much with WebForms in a long time, but I still use loose ASPX pages quite a lot for my static content. Say what you will about WebForms for free standing pages that might need a little bit of code for date stamping or retrieving some remote status, or simply can take advantage of master pages, ASPX is very low overhead in terms of what you need on a server - it's built into IIS after all. Nothing needs to be installed and it just works, which is why I often fall back to ASPX pages rather than WebPages or now RazorPages.

Creating this control was pretty easy. The process is:

* Subclass from a Literal control
* Override the OnRender() method
* Take the Text property input and turn it into Markdown
* Write out the Markdown in the HtmlText writer


It's literally just a few lines of code, plus some additional fix up for optionally normalizing white space on the markdown block.

Here's the code for the control:

```cs
[assembly: TagPrefix("Markdown", "ww")]

[DefaultProperty("Text")]
[ToolboxData("<{0}:Markdown runat=server></{0}:Markdown>")]
public class Markdown : System.Web.UI.WebControls.Literal
{
   
    [Description("Tries to strip whitespace before all lines based on the whitespace applied on the first line.")]
    [Category("Markdown")]
    public bool NormalizeWhiteSpace { get; set; } = true;
    
    protected override void Render(HtmlTextWriter writer)
    {
        if (string.IsNullOrEmpty(Text))
            return;

        string markdown = NormalizeWhiteSpaceText(Text);

        var parser = MarkdownParserFactory.GetParser(usePragmaLines: false, forceReload: false);
        var html = parser.Parse(markdown);
        writer.Write(html);
    }

    string NormalizeWhiteSpaceText(string text)
    {
        if (!NormalizeWhiteSpace || string.IsNullOrEmpty(text))
            return text;

        var lines = GetLines(text);
        if (lines.Length < 1)
            return text;

        string line1 = null;

        // find first non-empty line
        for (int i = 0; i < lines.Length; i++)
        {
            line1 = lines[i];
            if (!string.IsNullOrEmpty(line1))
                break;
        }

        if (string.IsNullOrEmpty(line1))
            return text;
        
        string trimLine = line1.TrimStart();
        int whitespaceCount = line1.Length - trimLine.Length;
        if (whitespaceCount == 0)
            return text;
        string whitespace = line1.Substring(0, whitespaceCount);


        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < lines.Length; i++)
        {
            sb.AppendLine(lines[i].Replace(whitespace, ""));
        }

        return sb.ToString();
    }

    static string[] GetLines(string s, int maxLines = 0)
    {
        if (s == null)
            return null;

        s = s.Replace("\r\n", "\n");

        if (maxLines < 1)
            return s.Split(new char[] { '\n' });

        return s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
    }



    /// <summary>
    /// Renders raw markdown from string to HTML
    /// </summary>
    /// <param name="markdown"></param>
    /// <param name="usePragmaLines"></param>
    /// <param name="forceReload"></param>
    /// <returns></returns>
    public static string Parse(string markdown, bool usePragmaLines = false, bool forceReload = false)
    {
        if (string.IsNullOrEmpty(markdown))
            return "";

        var parser = MarkdownParserFactory.GetParser(usePragmaLines, forceReload);
        return parser.Parse(markdown);
    }

    /// <summary>
    /// Renders raw Markdown from string to HTML.
    /// </summary>
    /// <param name="markdown"></param>
    /// <param name="usePragmaLines"></param>
    /// <param name="forceReload"></param>
    /// <returns></returns>
    public static HtmlString ParseHtmlString(string markdown, bool usePragmaLines = false, bool forceReload = false)
    {
        return new HtmlString(Parse(markdown, usePragmaLines, forceReload));
    }

}
```    

The key is the `Render()` method which text the markdown content from the `Text` property and converts it to HTML and then writes out the raw content into the HTML text writer.

##AD##

## Dealing with White Space
`NormalizeWhitespace` deals with stripping leading white space from the markdown text in a control. What this means that when you embed Markdown like this:

```html
<ww:Markdown runat="server" id="md2" 
			 NormalizeWhiteSpace="True">
    # Markdown Monster Change Log 
    [download latest version](https://markdownmonster.west-wind.com/download.aspx) &bull; 
    [install from Chocolatey](https://chocolatey.org/packages/MarkdownMonster) &bull; 
    [Web Site](https://markdownmonster.west-wind.com)
</ww:Markdown>
```

the leading spaces in that block are stripped. If they weren't the above would simply render as code (ie. more than leading whitespace 4 characters). As an alternative you can write your markdown left aligned to the 0 margin:

```html
<ww:Markdown runat="server" id="md2" NormalizeWhiteSpace="False">
# Markdown Monster Change Log 
[download latest version](https://markdownmonster.west-wind.com/download.aspx) &bull; 
[install from Chocolatey](https://chocolatey.org/packages/MarkdownMonster) &bull; 
[Web Site](https://markdownmonster.west-wind.com)
</ww:Markdown>
```

which guarantees that the Markdown is rendered as is.

`NormalizeWhiteSpace` is `true` by default so typically it does what you'd expect it to do. Use `false` if your leading spaces are wonky (ie. not the same for all lines) and left justify to the left margin.

## Sanitizing HTML
Markdown is essentially a superset of HTML as you can embed any HTML into Markdown. Markdown itself doesn't have any rules about what HTML can be embedded and it's entirely possible to embed script code inside of markdown.

> #### @icon-warning Markdown and Script Attacks
> If you accept Markdown as user input, you have to treat Markdown with the same security concerns as you would raw HTML input. 

If you capture user Markdown input you have to ensure you sanitize the rendered HTML and remove any potential code execution.

To help with this, the Markdown control has a `SanitizeHtml` property which is **set to `True` by default** and which performs rudimentary script sanitation. It removes `<script>`, `<iframe>`, `<form>` and a few other elements, removes `javascript:` and `data:` attribute content, and removes `onXXX` event handlers from HTML input.

If you rather render your Markdown *as is* set `SantizeHtml` to `False` which has to be done explicitly to get raw, unsanitized HTML output. 

To see what that looks like you can try the following in a Markdown block. Add this to your page and switch `SanitizeHtml` between `true` and `false`.

```html
<markdown runat="server" id="mm1" 
          SanitizeHtml="False">
	
	### Links:
	[Please don't hurt me](javascript:alert('clicked!');)
	
	### Script Blocks
	<script>alert('this will show!');</script>
	
	<div onmouseover="alert('That really hurts!')"
	     style="opacity: 0; padding: 20px;">
		A hidden menace in Venice
	</div>
</markdown>
```

##AD## 

## Markdown Conversion
The control also allows raw Markdown Parsing using plain code with a couple of static helper methods, so you can do the following:

```cs
string html = Markdown.Parse("This is **a very bold Markdown**.");
```

or by directly embedding into ASPX pages:

```cs
<%= Markdown.Parse("This is **a very bold Markdown**.") %>
```
It works elsewhere too (like MVC), but you're not likely to add the control to non WebForms projects. But if for some reason you do, there's another version of the `Parse` method as `ParseHtml()` which returns a `HtmlString` instance instead that you can use in `@Markdown.ParseHtml("This is **a very bold Markdown**.")` in WebPages or MVC.

#### sanitizeHtml Parameter
By default the Parse methods apply HTML sanitation via a `sanitzeHtml` parameter, which defaults to `true`. If you would like to get the raw unsanitized HTML returned or you want to do your own HTML Sanitation post parsing, set `sanitizeHtml: false` in the method call.

For code you know is safe turn sanitation off:

```cs
string html = Markdown.Parse(staticMarkdown,sanitizeHtml: false);
```

For user input that you echo back to the screen make sure you sanitize:

```cs
// true is the default but it's good to be explicit!
string html = Markdown.Parse(staticMarkdown, sanitizeHtml: true);
```

## Parsing Markdown
All of this functionality is really made possible by the Markdown parser which in this case is the excellent [Markdig](https://github.com/lunet-io/markdighttps://github.com/lunet-io/markdig) library. I tend to wrap whatever MarkdownParser I use in a small factory interface as over the last few years I've switched parsers quite frequently.

This interface provides a MarkdownParser and MarkdownParserFactory to get instances of the parser and parsing the text.

The Factory's main responsibility is for caching the parser instance so it's not created each and every time a request for parsing is made:

```cs
public static class MarkdownParserFactory
{
    /// <summary>
    /// Use a cached instance of the Markdown Parser to keep alive
    /// </summary>
    public static IMarkdownParser CurrentParser;

    /// <summary>
    /// Retrieves a cached instance of the markdown parser
    /// </summary>                
    /// <param name="forceLoad">Forces the parser to be reloaded - otherwise previously loaded instance is used</param>
    /// <param name="usePragmaLines">If true adds pragma line ids into the document that the editor can sync to</param>
    /// <param name="parserAddinId">optional addin id that checks for a registered Markdown parser</param>
    /// <returns>Mardown Parser Interface</returns>
    public static IMarkdownParser GetParser(bool usePragmaLines = false,
                                            bool forceLoad = false, string parserAddinId = null)
    {
        if (!forceLoad && CurrentParser != null)
            return CurrentParser;
        
        CurrentParser = new MarkdownParserMarkdig(usePragmaLines: usePragmaLines, force: forceLoad);

        return CurrentParser;
    }
}
```    

The `MarkdownParserMarkdig class then handles instantiating the parser including all the extension that should be loaded:

```cs
protected virtual MarkdownPipelineBuilder CreatePipelineBuilder()
{
    var builder = new MarkdownPipelineBuilder()
        .UseEmphasisExtras()
        .UsePipeTables()
        .UseGridTables()
        .UseFooters()
        .UseFootnotes()
        .UseCitations();


        builder = builder.UseAutoLinks();
        builder = builder.UseAutoIdentifiers();
        builder = builder.UseAbbreviations();
        builder = builder.UseYamlFrontMatter();
        builder = builder.UseEmojiAndSmiley();
        builder = builder.UseMediaLinks();
        builder = builder.UseListExtras();
        builder = builder.UseFigures();
        builder = builder.UseTaskLists();
        //builder = builder.UseSmartyPants();            

    if (_usePragmaLines)
        builder = builder.UsePragmaLines();

    return builder;
}
```        

For now this code hard codes the extensions used which are the most common extensions typically used. However, it would be nice to eventually support overriding these either by allowing a hook event or by using the `builder.Configure()` method to pass in a list of extensions that should be used. But I'll leave that for another day.

Finally, the Markdown still needs to be parsed. Parsing Markdown with Markdig is easy enough:

```cs
public override string Parse(string markdown)
{
    if (string.IsNullOrEmpty(markdown))
        return string.Empty;

    var htmlWriter = new StringWriter();
    var renderer = CreateRenderer(htmlWriter);

    Markdig.Markdown.Convert(markdown, renderer, Pipeline);

    var html = htmlWriter.ToString();
    
    html = ParseFontAwesomeIcons(html);

    //if (!mmApp.Configuration.MarkdownOptions.AllowRenderScriptTags)
    html = ParseScript(html);  
              
    return html;
}
```        

It probably would be a good idea to also have an overload that takes an HTML Text Writer to directly write the output but I do some optional post processing for FontAwesomeIcons (@icon-faIconName) that requires converting to HTML anyway.

The parse method is what's then called by the Web control which gets a reference to an `IMarkdownParser` and calls the `Parse()` interface method.

It works and is also very efficient. Markdig is kick ass in performance and has nearly unnoticable overhead when hitting pages.

Sweet.

## Summary
None of this is rocket science or anything new. But this little control fits a need I've been meaning to address for a while and I finally spent a couple of hours throwing it together. 

I've put the code on Github and the control into Nuget so if this sounds like a use case you run into from time to time - have at it.


## Resources
* [A Markdown TagHelper and Markdown Parser for ASP.NET Core](https://weblog.west-wind.com/posts/2018/Mar/23/Creating-an-ASPNET-Core-Markdown-TagHelper-and-Parser)
* [Creating a generic Markdown Page Handler using ASP.NET Core Middleware](https://weblog.west-wind.com/posts/2018/Apr/18/Creating-a-generic-Markdown-Page-Handler-using-ASPNET-Core-Middleware)
* [Markdown and Cross-Site Scripting](https://weblog.west-wind.com/posts/2018/Aug/31/Markdown-and-Cross-Site-Scripting)
* [Westwind.Web.Markdown on GitHub](https://github.com/RickStrahl/Westwind.Web.Markdown)
* [Westwind.Web.Markdown on Nuget](https://www.nuget.org/packages/Westwind.Web.Markdown)


<div style="margin-top: 30px;font-size: 0.8em;
            border-top: 1px solid #eee;padding-top: 8px;">
    <img src="https://markdownmonster.west-wind.com/favicon.png"
         style="height: 20px;float: left; margin-right: 10px;"/>
    this post created and published with 
    <a href="https://markdownmonster.west-wind.com" 
       target="top">Markdown Monster</a> 
</div>