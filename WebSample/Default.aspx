<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="Default.aspx.cs" Inherits="_Default" ValidateRequest="false" %>
<%@ Register TagPrefix="ww" Namespace="Westwind.Web.Markdown" Assembly="Westwind.Web.Markdown"  %>
<asp:Content runat="server"  ContentPlaceHolderID="MainContent">
<div class="container">
<h1>Markdown Test Page</h1>
    
    <form id="form1" runat="server" >
        <div>
            
            <h3 class="header" >Markdown with Normalized Text (default, doesn't have to be left-aligned):</h3>
            
            
            <div class="small margin-bottom-2x margin-top" style="color: #bbb">rendered Markdown starts below...</div>
            
            


            <ww:Markdown runat="server" id="Markdown2" >
                # Markdown Monster Change Log 
                <small> [download latest version](https://markdownmonster.west-wind.com/download.aspx) &bull; 
                    [install from Chocolatey](https://chocolatey.org/packages/MarkdownMonster) &bull; 
                    [Web Site](https://markdownmonster.west-wind.com)</small>


                ### 1.6.2
                *<small>September 10th, 2017</small>*

                * **Keyboard support for Context Menu**  
                You can now pop up the context menu via keyboard using the Windows context menu key (or equivalent). The menu is now cursor navigable. This brings spell checking and various edit operations to keyboard only use.

                * **Fix: `UseSingleWindow=false` no longer opens Remembered Documents**   
                When not running in `UseSingleWindow` mode, the `RememberLastDocumentsLength` setting has no effect and no previous windows are re-opened. This is so multiple open windows won't open the same documents all the time. In `UseSingleWindow` mode last documents are remembered and opened when starting MM for the first time.

                * **Fix: YAML parsing for Blog Post**  
                Fixed bug where the YAML header on the meta data was not properly inserting a line break after the parsed YAML block.

                ### 1.6
                *<small>September 6th, 2017</small>*

                * **Version Rollup Release**   
                This release is a version rollup release that combines all the recent additions into a point release.

                * **Edit and Remove Hyperlink Context Menu Options**  
                Added menu options to edit hyper links in the link editor or to remove the hyperlink and just retain the text of the link.

                * **Fix: Command Refactoring**  
                The various Command objects used to define menu options have been refactored in the code with seperate configuration wrapper methods to make it easier to find and edit the menu options.

                * **Fix: Addin Loading Issue**  
                Looks like a regression bug slipped through in 1.5.12 that would not allow loading of certain addins (Gist, Pandoc addins specifically).
            </ww:Markdown>

            
            <h3 class="header">Markdown Display (non-normalized, has to be left aligned):</h3>
          
            <hr />
                
            
            <ww:Markdown runat="server" id="md1" NormalizeWhiteSpace="false">
### Eeeer, hold on there Doc: ASP.NET Core on IIS
If you try this on a site that uses IIS with the ASP.NET Core module configured, you're going to find that this process doesn't work - you'll get an error to the effect that the Validation File returned HTML instead of the desired validation id:

![](ValidationFailed.png)

The reason is that Lets Encrypt serves **an extensionless file out of a special folder** it creates off the root of the Web site:

```
/.well-known/acme-challenge/xxxxxxXXXXXX
```

By default your ASP.NET Core application doesn't have a way to serve this file as ASP.NET doesn't recognize it as a static file but handles it as an extensionless URL.

LetsEncrypt-Win-Simple uses a custom `web.config` file in the `.well-known` folder to allow access to the extensionless file that is stored at that endpoint. Unfortunately, when running IIS with ASP.NET Core, **all** requests are routed directly into the ASP.NET Core Module and therefore your application.

Which results in:

![](Fail.jpg)

<small>**No Soup for you!**</small>

There are a number of ways you can fix this from temporarily disabling the AspNetCore Module, to adding some code in your Asp.NET Core application configuration to handle the special Urls.

Lets take a look at some of the approaches.

### 4. Use a Controller Action with Custom Route
Another alternative is do the same thing using a controller method with an attribute route:

```cs
[HttpGet]
[Route(".well-known/acme-challenge/{id}")]
public ActionResult LetsEncrypt(string id)
{
    var file = Path.Combine(this.HostingEnv.WebRootPath, ".well-known", "acme-challenge", id);
    return PhysicalFile(file, "text/plain");            
}
```
            </ww:Markdown>
            
            
            <h3 class="header">SanitizeHtml Option</h3>
            <hr />

            <ww:Markdown runat="server" SanitizeHtml="True">
                <h5>There's a script hiding below (shouldn't fire alert box on load!)</h5>
                
                <script>
                    alert('GOTCHA! XSS access');
                </script>
                
                <h5>Link with `javascript:` embedded (link shouldn't work):</h5>
                <a href="javascript:alert('GOTCHA: javascript: code fired')">Click me - I'm not evil</a>
                
                <h5>Element with <code>onmouseover</code> (shouldn't activate when you hover over):</h5>                
                <div class="alert alert-info" style="padding: 20px" onmouseover="alert('GOTCHA: onmouseoverevent fired')">Don't hurt me!</div>                

            </ww:Markdown>
                
            <h3 class="header">Dynamically assign Markdown:</h3>
            <hr />
                
            <label class="">Enter some Markdown:</label><br />
            <asp:TextBox runat="server" ID="EditedMarkdown" TextMode="Multiline" class="form-control" style="height: 245px;">#### Markdown Text
Write some markdown text and see if it sticks.

* Lines
* Lines
* Lines

[created with Markdown Monster](https://markdownmonster.west-wind.com)
                
<script>
    alert("GOTCHA: You shouldn't see this script with SanitizeHtml on");
</script>
            </asp:TextBox>
            <asp:Button runat="server" ID="btnSubmit" Text="Save" />
            <hr />

            <%= Markdown.Parse(EditedMarkdown.Text, sanitizeHtml: true) %>
        
        </div>
        
        
        

    </form>
</div>
</asp:Content>