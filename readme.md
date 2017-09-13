# Westwind.Web.MarkdownControl
#### A basic Markdown content rendering control for ASP.NET Web Forms

This simple project provides an ASP.NET Web Forms Server control that allows you to embed Markdown as literal text into a page and expand it as the page renders. This is useful for content pages that contain lots of text and when you don't want to write out HTML in these pages. 

Instead you can add a control like this into the page:

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

### Features
The control provides these features:

* ASP.NET Server Control `<ww:Markdown></ww:Markdown>`
* Ability to normalize leading spacing
* Includes a static method to parse Markdown
* Uses the awesome [MarkDig Markdown parser](https://github.com/lunet-io/markdig) to parse Markdown

### Control Usage and Syntax
To use the control add it to your page like any other server control.

First add a reference to the control assembly either on the page or in Web.config:

At the top of the page:
```html
<%@ Register TagPrefix="ww" Namespace="Westwind.Web.MarkdownControl" Assembly="Westwind.Web.MarkdownControl" %>
```

or in `web.config` globally:

```xml
<configuration>
    <system.web>
        <pages>
            <controls>
                <add assembly="Westwind.Web.MarkdownControl" 
                     namespace="Westwind.Web.MarkdownControl" 
                     tagPrefix="ww" />
            </controls>
        </pages>
    </system.web>
</configuration
```      

Then embed the control into the page where you want the markdown to appear:

```xml
<ww:Markdown runat="server" id="md2" NormalizeWhiteSpace="True">
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

Notice the indentation of the markdown. If not normalized the entire markdown block would render as code (more than 4 white space characters which is a code block). When NormalizeWhiteSpace is true the leading space is stripped of the entire block.

> #### @icon-info-circle First Line Determines Whitespace to strip
> The first line of the Markdown block determines what white space is stripped from all other lines.

Note the default is `True` - if you have funky behavior due to indentation I'd recommend you left justify your markdown and set this value to `False`.


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

