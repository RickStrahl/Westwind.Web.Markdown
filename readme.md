# Westwind.Web.Markdown
#### Markdown Support for your System.Web MVC and WebForms applications

This library provides:

* **Markdown Parser Functions**  
Simple Markdown Parsing with `Markdown.Parse()` and `Markdown.ParseHtml()` methods you can use in code or in your MVC or WebForms markup.

* **Markdown Web Control for 'Markdown Islands'**  
A useful `<markdown>` server WebControl for easily embedding static Markdown text or data-bound Markdown into a document. Great for using Markdown for static content inside of larger Web Pages.

* **Serve Markdown Pages from Disk using MarkdownHttpHandler**  
This Http handler allows you to drop Markdown files into a site and serve those Markdown files as HTML rendered into a configurable template. The template's job is to provide the site chrome around the HTML so this can simply be a small content page pointing a Master Page in WebForms or Layout Page in MVC. A model object is passed via the Items collection.

* **Uses the the awesome Markdig Parser**  
This library relies on the [Markdig](https://github.com/lunet-io/markdig) Markdown Parser and provides access to all Markdown Pipeline configuration options of that parser via a configuration hook.

* **Optional Html Script Sanitation**  
Both the parsers and Web Control have options to sanitize HTML to avoid common XSS attacks.

## Static Markdown Parsing Functions
For direct Markdown parsing you can use this library for rendering Markdown to HTML as strings or `HtmlString` values for MVC.

```cs
string html = Markdown.Parse("This is **bold Markdown**.");
```

or

```html
<%= Markdown.Parse("This is **bold Markdown**.") %>
```

or even this in WebPages or MVC:

```cs
@Markdown.ParseHtml("This is **bold Markdown**.")
```


## Markdown Control
The Web Control provides you the abililty to easily embed static Markdown text into any Web Forms page.

```html
<ww:Markdown runat="server" id="md1">
    ### 1.6.2
    *September 10th, 2017*
    
    * **Keyboard support for Context Menu**  
    You can now pop up the context menu via keyboard using the Windows context menu key (or equivalent). The menu is now cursor navigable. This brings spell checking and various edit operations to keyboard only use.
    
    * **Fix: `UseSingleWindow=false` no longer opens Remembered Documents**   
    When not running in `UseSingleWindow` mode, the `RememberLastDocumentsLength` setting has no effect and no previous windows are re-opened. This is so multiple open windows won't open the same documents all the time. In `UseSingleWindow` mode last documents are remembered and opened when starting MM for the first time.
</ww:Markdown>
```

And the content will be rendered to HTML at runtime.


## Get it from NuGet
To use the control you can install from [Nuget](https://www.nuget.org/packages/Westwind.Web.Markdown/):

```ps
PS> install-package Westwind.Web.Markdown
```

## Control Usage and Syntax
To use the control add it to your page like any other server control.

First add a reference to the control assembly either on the page or in Web.config:

At the top of the page:
```html
<%@ Register TagPrefix="ww" Namespace="Westwind.Web.Markdown" Assembly="Westwind.Web.Markdown" %>
```

or in `web.config` globally:

```xml
<configuration>
    <system.web>
        <pages>
            <controls>
                <add assembly="Westwind.Web.Markdown" 
                     namespace="Westwind.Web.Markdown" 
                     tagPrefix="ww" />
            </controls>
        </pages>
    </system.web>
</configuration
```      

Then embed the control into the page where you want the markdown to appear:

```xml
<ww:Markdown runat="server" id="md2" 
             NormalizeWhiteSpace="True"
             SanitizeHtml="True">
    # Markdown Monster Change Log 
    [download latest version](https://markdownmonster.west-wind.com/download.aspx) &bull; 
    [install from Chocolatey](https://chocolatey.org/packages/MarkdownMonster) &bull; 
    [Web Site](https://markdownmonster.west-wind.com)
</ww:Markdown>
```

#### NormalizeWhiteSpace
This property is true by default and if the control starts with a line that is indented it will strip the same indentation from all lines following. This allows text like this to render properly as Markdown:

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

Notice the indentation of the markdown.  With `NormalizeWhiteSpace` off you need to explicitly left align the embedded Markdown Content:

```xml
<ww:Markdown runat="server" id="md2" NormalizeWhiteSpace="False">
# Markdown Monster Change Log 
[download latest version](https://markdownmonster.west-wind.com/download.aspx) &bull; 
[install from Chocolatey](https://chocolatey.org/packages/MarkdownMonster) &bull; 
[Web Site](https://markdownmonster.west-wind.com)
</ww:Markdown>
```

If not normalized the entire markdown block would render as code (more than 4 white space characters which is a code block). When `NormalizeWhiteSpace` is true the leading space is stripped of the entire block.

> #### First Line Determines Whitespace to strip
> The first line of the Markdown block determines what white space is stripped from all other lines.

Note the default is `True` - if you have funky behavior due to indentation I'd recommend you left justify your markdown and set this value to `False`.

#### SanitizeHtml
Markdown is essentially a superset of HTML as you can embed any HTML into Markdown. Markdown itself doesn't have any rules about what HTML can be embedded and it's entirely possible to embed script code inside of markdown.

If you capture Markdown text from users it's important **you treat input Markdown just as you would raw HTML user input**.

To help with this, the Markdown control has a `SanitizeHtml` property which is **set to `True` by default**, which performs rudimentary script sanitation. It removes `<script>`, `<iframe>`, `<form>` and a few other elements, removes `javascript:` and `data:` attribute content, and removes `onXXX` event handlers from HTML input.

If you rather render your Markdown *as is* set `SantizeHtml` to `False`. To see what that looks like you can try the following in your Markdown block:


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

Both `Markdown.Parse()` and `Markdown.ParseHtml()` also have a sanitizeHtml parameter that is `true` by default.

If you render static text you control then `SanitizeHtml=False` is usually Ok, but if you take user input and put into the browser to display, **always use `SanitizeHtml=True`**.

### Static Markdown Rendering
The control also includes static Markdown rendering that you can use in your Web Application inside of pages or your Web code.

```cs
string html = Markdown.Parse("This is **bold Markdown**");
```

You can also embed Markdown into pages like this:

```html
<div class="container" id="ChangeLogText">
    <%= Markdown.Parse(Model.ChangelogMarkdownText) %>
</div>    
```

or in WebPages or MVC:


```html
<div class="well well-sm">
    @Markdown.ParseHtml("This is **bold Markdown**.")
</div>
```

#### sanitizeHtml Parameter
By default the Parse method applies HTML sanitation via a `sanitzeHtml` parameter, which defaults to `true`. If you would like to get the raw unsanitized HTML returned or you want to do your own HTML Sanitation post parsing, set `sanitizeHtml: false` in the method call.

For code you know is safe:

```cs
string html = Markdown.Parse(staticMarkdown,sanitizeHtml: false);
```

For user input that you echo back to the screen:

```cs
// true is the default but it's good to be explicit!
string html = Markdown.Parse(staticMarkdown, sanitizeHtml: true);
```

> #### Important
> Always treat user entered Markdown as you would raw HTML!



## Markdown Page Handler
The Markdown HTTP Handler allows you to simply drop Markdown files into an ASP.NET Web site and get those pages served as HTML. You can provide a template to provide the site's chrome around the rendered and access a 'model' that contains the title, the rendered Markdown,  original Markdown and a few other things to render into your template.

This feature works with WebForms or MVC based applications.

To do this you need to:

* Add a reference to Westwind.Web.Markdown Nuget
* Add a Handler mapping of `.md` to `MarkdownHttpHandler`
* Set up a 'template' HTML page or View
* Use `Context.Items[MarkdownHttpHandler.ItemKey] to retrieve a model
* Embed `Model.RenderedMarkdownHtml` into the template

### Add an HttpHandler Mapping
In order for IIS and ASP.NET to process `.md` (or whatever other extensions you choose) files, the extension has to be registered in `web.config`.

```xml
<configuration>
  <system.webServer>
    <handlers>      
      <add name=".md extension" 
           path="*.md" verb="GET" 
           type="Westwind.Web.Markdown.MarkdownHttpHandler,Westwind.Web.Markdown" 
           preCondition="integratedMode" />
    </handlers>
  </system.webServer>
</configuration>
```

### Configuration 
You can optionally configure the handler's operation using the static configuration object provided as `MarkdownHttpHandler.Configuration`. Preferrably you'll want to set this configuration once during application startup in `Application_Start()`:

```cs
void Application_Start(object sender, EventArgs e) 
{
	MarkdownHttpHandler.Configuration.SanitizeHtml = true;
	MarkdownHttpHandler.Configuration.MarkdownTemplatePagePath = "~/_MarkdownPage.aspx";
}
```

Both of these values are shown as default above so unless you need to change those values you don't have to set them. The important one is the virtual path to the template that will actually render the Markdown content as a full HTML document. This can be any valid page in your `System.Web` based Web site that can access the `Context.Items` collection which is needed to retrieve the model data.

### Create a Template
Next you'll need a template into which the markdown content can be rendered. Remember Markdown is just an **HTML Fragment** not a full document, so Markdown always needs a host document. Most likely you'll also want to make sure the document renders consistently using your Web site's consistent site chrome.

You can use either WebForms or MVC to do this - just pick an WebForms page or MVC view as an endpoint and then access the following model data in your HTML markdown or codebehind/controller:

```cs
var model = Context.Items[MarkdownHttpHandler.ModelKey] as MarkdownModel;
```

The model's properties available look like this:

| Property         | Value                                    |
|------------------|------------------------------------------|
| Title            | The title of the page from first # header or YAML |
| RenderedMarkdown | An HtmlString value that contains the rendered Markdown |
| RawMarkdown      | Holds the raw, original Markdown text    |
| YamlHeader       | Hold the YAML header if one is provided  |
| RelativePath     | The relative virtual path of the original Markdown File |
| PhysicalPath     | The physical path of the original Markdown File requested |


Most likely the only values you'll be interested in are `RenderedMarkdown` and the `Title`. You'll want to the title

#### WebForms

```html
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
```

#### MVC
For MVC you'll want to change the path to point a view/controller of your choice in the configuration in `Application_Start()`:

```cs
// this will be an internally redirected URL only
MarkdownHttpHandler.Configuration.MarkdownTemplatePagePath = "~/system/markdownhandling";
```

Then you can have a controller method (or just code in a view):

```
public ActionResult MarkdownHandling()
{
	var model = Context.Items[MarkdownHttpHandler.ModelKey] as MarkdownModel;
	return View(model);
}
```

Then inside of the Razor view you can utilize the model as needed:

```html
@model MarkdownModel

<div class="container">
	<div class="page-header">
		@model.Title
	</div>

	<div class="page-content">
		@model.RenderedMarkdown
	</div>
</div>
```

> If you need to pass other items into the view like authentication or login data required in your site chrome, you can just create a custom view that includes theMarkdownModel data. I left this in raw form from the `Context.Items` collection to allow this to work in just about any `System.Web` based solution. 

### Dropping Files into your Site
At this point you can just drop `.md` files into your site. The files should be routed to the `MarkdownHttpHandler` which in turn renders the Markdown to HTML and calls your template with the `Context.Items` item that contains the model, which is then rendered by your customized template.

This is great to add some documentation or other text heavy content to your site.


## Customizing the Markdown Pipeline
This parser uses the MarkDig Markdown parser which supports creating a custom pipeline. By default the parser is configured with most add-on features enabled. if you want to explicitly customize this list - either to minimize for performance, or for additional features you can override the static `MarkdownParserMarkdig.OnCreateMarkdigPipeline` function during application startup.

When this `Func<bool,MarkdigPipeline>` is set, this function is called instead of the default pipeline build logic.

Call this during application startup since the parser gets cached after first access. A good place as part of `Application_Init` processing:

```cs
void Application_Start(object sender, EventArgs e)
{
    // OPTIONAL - override parser pipeline addins
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

}
```

Note that the parser is cached so if you change this value anywhere but in startup code, you can explicitly force the parser to refresh with:

```cs
Markdown.Parse("ok",forceReload: true);
```

### Adding Code Highlighting
If you'd like to highlight your code snippets with syntax highlighting I recommend [Highlight.js](https://highlightjs.org/). Using this easy to use library you can add the following to a page to get syntax coloring for code snippets:

```html
<link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/styles/dracula.min.css" />
<script src="//cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/highlight.min.js"></script>

<script>
    function highlightCode() {
        var pres = document.querySelectorAll("pre>code");
        for (var i = 0; i < pres.length; i++) {
            hljs.highlightBlock(pres[i]);
        }
    }
    highlightCode();
</script>
```

## License
The Westwind.Web.MarkdownControl library is an open source product licensed under:

* **[MIT license](http://opensource.org/licenses/MIT)**

All source code is **&copy; West Wind Technologies**, regardless of changes made to them. Any source code modifications must leave the original copyright code headers intact if present.

There's no charge to use, integrate or modify the code for this project. You are free to use it in personal, commercial, government and any other type of application and you are free to modify the code for use in your own projects.

### Give back
If you find this library useful, consider making a small donation:

<a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=3RZRUFCWD24R4" 
    title="Find this library useful? Consider making a small donation." alt="Make Donation" style="text-decoration: none;">
	<img src="https://weblog.west-wind.com/images/donation.png" />
</a>

## Acknowledgements
This library uses the following excellent components:

* [MarkDig Markdown Parser](https://github.com/lunet-io/markdig)

---

<div style="margin-top: 30px;font-size: 0.8em;
            border-top: 1px solid #eee;padding-top: 8px;">
    <img src="https://markdownmonster.west-wind.com/favicon.png"
         style="height: 20px;float: left; margin-right: 10px;" height="20" width="20" />
    content created and published with 
    <a href="https://markdownmonster.west-wind.com"
       target="top">Markdown Monster</a>
</div>

