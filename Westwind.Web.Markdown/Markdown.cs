#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2016
 *          http://www.west-wind.com/
 * 
 * Created: 04/28/2016
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion


using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Westwind.Web.Markdown.MarkdownParser;

[assembly: TagPrefix("Markdown", "ww")]

namespace Westwind.Web.Markdown
{    
    /// <summary>
    /// An ASP.NET Server control that renders the embedded text as Markdown.
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Markdown runat=server></{0}:Markdown>")]
    public class Markdown : System.Web.UI.WebControls.Literal
    {

        /// <summary>
        /// Tries to strip whitespace before all lines based on the whitespace applied on the first line.
        /// </summary>
        [Description("Tries to strip whitespace before all lines based on the whitespace applied on the first line.")]
        [Category("Markdown")]
        public bool NormalizeWhiteSpace { get; set; } = true;

        /// <summary>
        /// Strips scriptable tags and attributes. Minimal implementation.
        /// </summary>
        [Description("Strips scriptable tags and attributes. Minimal implementation.")]
        [Category("Markdown")]

        public bool SanitizeHtml { get; set; } = true;


        /// <summary>
        /// Virtual site relative file path that is loaded and displayed in the control's
        /// content.
        ///
        /// ~/Markdown/Site
        ///
        /// Note related resources inside of the Markdown are *host page relative*, not Markdown document relative.
        /// </summary>
        [Description("Optional file name from which to load the Markdown. Overrides content setting if set. Note: markdown related resources are *HTML host page relative* not relative to the markdown document.")]
        [Category("Markdown")]
        public string Filename { get; set; } = null;

        /// <summary>
        /// Overrides the HTML rendering of content
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {

            string markdown = null;

            if (!string.IsNullOrEmpty(Filename))
            {
                var file = Context.Server.MapPath(Filename);                
                try
                {
                    markdown = File.ReadAllText(file);
                }
                catch
                {
                    return;
                }
            }
            else
            {

                if (string.IsNullOrEmpty(Text))
                    return;

                if (NormalizeWhiteSpace)
                    markdown = NormalizeWhiteSpaceText(Text);
                else
                    markdown = Text;
            }

            var parser = MarkdownParserFactory.GetParser(false, false);
            var html = parser.Parse(markdown,SanitizeHtml);
            writer.Write(html);
        }

        private string NormalizeWhiteSpaceText(string text)
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

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > whitespaceCount)
                    sb.AppendLine(lines[i].Substring(whitespaceCount));
                else
                    sb.AppendLine(lines[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses a string into an array of lines broken
        /// by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <param name="maxLines">Optional - max number of lines to return</param>
        /// <returns>array of strings, or null if the string passed was a null</returns>
        static string[] GetLines(string s, int maxLines = 0)
        {
            if (s == null)
                return null;

            s = s.Replace("\r\n", "\n");

            if (maxLines < 1)
                return s.Split(new char[] { '\n' });

            return s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
        }




        #region Static Helpers 

        /// <summary>
        /// Renders raw markdown from string to HTML
        /// </summary>
        /// <param name="markdown">Markdown to Render</param>
        /// <param name="usePragmaLines">Generates line numbers as ids into headers and paragraphs. Useful for previewers to match line numbers to rendered output</param>
        /// <param name="forceReload">Forces the parser to be reloaded completely rather than using a cached instance</param>
        /// <param name="sanitizeHtml">Strips out scriptable tags and attributes for prevent XSS attacks. Minimal implementation.</param>
        /// <returns></returns>
        public static string Parse(string markdown, bool usePragmaLines = false, bool forceReload = false, bool sanitizeHtml = true)
        {
            if (string.IsNullOrEmpty(markdown))
                return "";

            var parser = MarkdownParserFactory.GetParser(usePragmaLines, forceReload);
            return parser.Parse(markdown,sanitizeHtml);
        }

        /// <summary>
        /// Renders raw Markdown from string to HTML with an HTMLString output for use in MVC or Web Pages instead of Raw output.
        /// </summary>
        /// <param name="markdown">Markdown to render</param>
        /// <param name="usePragmaLines">Generates line numbers as ids into headers and paragraphs. Useful for previewers to match line numbers to rendered output</param>
        /// <param name="forceReload">Forces the parser to reloaded. Otherwise cached instance is used</param>
        /// <param name="sanitizeHtml">Strips out scriptable tags and attributes for prevent XSS attacks. Minimal implementation.</param>
        /// <returns></returns>
        public static HtmlString ParseHtmlString(string markdown, bool usePragmaLines = false, bool forceReload = false, bool sanitizeHtml = true)
        {
            return new HtmlString(Parse(markdown, usePragmaLines, forceReload, sanitizeHtml));
        }

        #endregion
    }
}
