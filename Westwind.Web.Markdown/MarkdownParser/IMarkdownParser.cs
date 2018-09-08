namespace Westwind.Web.Markdown.MarkdownParser
{
    public interface IMarkdownParser
    {
        /// <summary>
        /// Returns parsed markdown
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="sanitizeHtml">Sanitizes generated HTML by stripping scriptable html markup and directives</param>
        /// <returns></returns>
        string Parse(string markdown, bool sanitizeHtml=true);
    }
}
