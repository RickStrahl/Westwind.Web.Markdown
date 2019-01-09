# Westwind.Web.Markdown Changelog

## Version 0.2.12

* **Add Markdown.ParseHtmlFromFile() and ParseHtmlStringFromFile()**  
Added methods that can parse Markdown from a file and embed it into the page.

## Version 0.2.10
*November 20th, 2018*

* **Add File Loading to Markdown Control**  
Added a `Filename` property to the Markdown control that allows specifying a virtual path to a Markdown document (such as `<ww:markdown Filename="~/EmbeddedMarkdownFile.md"`). This allows for easy referencing of large Markdown blocks without having to type them directly into the host document.

### Version 0.2.8
*October 30th, 2018*

* **Add MarkdownPageHandler to render Markdown Documents from Disk**  
Created a new Markdown handler that can serve Markdown pages from disk and use a template to render the HTML content as a complete page. The idea is to allow you to drop Markdown files into any Web site and render them as full HTML documents using your site's chrome.

## Version 0.2.7
*September 8th, 2018*

* **Renamed to Westwind.Web.Markdown**  
Renamed project to indicate wider Markdown feature scope than just the Markdown control.


## Version 0.2.5
*September 7th, 2018*

* **Add Explicit Html Sanitiation Support**  
Add `SanitizeHtml` property to the Markdown Control and `sanitizeHtml` parameters to the `Markdown.Parse()` and `Markdown.ParseHtml()` methods. Improved sanitation logic to capture more common XSS attack issues and unlike before, option to disable HTML Sanitation (it was always on before and still is on by default).

* **Add Markdig Pipeline Configuration Handler**  
There's now a `MarkdownParserMarkdig.OnCreateMarkdigPipeline` handler hook you can use to explicitly override the Markdown Parser configuration. You can override creation of the Markdig pipeline by specifying exactly the handlers you need to have processing.