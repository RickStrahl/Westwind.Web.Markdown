# Westwind.Web.Markdown Changelog

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