
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Westwind.Web.MarkdownControl.MarkdownParser;

[assembly: TagPrefix("Markdown", "ww")]

namespace Westwind.Web.MarkdownControl
{    
    /// <summary>
    /// An ASP.NET Server control that renders the embedded text as Markdown.
    /// </summary>
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

            var parser = MarkdownParserFactory.GetParser(false, false);
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

        #endregion
    }
}
