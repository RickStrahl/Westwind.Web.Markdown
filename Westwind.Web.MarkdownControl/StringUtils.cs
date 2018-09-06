using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Westwind.Web.Markdown.Utilities
{
    #region License

    /*
     **************************************************************
     *  Author: Rick Strahl 
     *          (c) West Wind Technologies, 2008 - 2018
     *          http://www.west-wind.com/
     * 
     * Created: 09/08/2008
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

    /// <summary>
    /// String utility class that provides a host of string related operations
    /// 
    /// Extracted from: westwind.utilitie
    /// </summary>
    public static class MarkdownUtils
    {              

        static string HtmlSanitizeTagBlackList { get; } = "script|iframe|object|embed|form";

        static Regex _RegExScript = new Regex($@"(<({HtmlSanitizeTagBlackList})\b[^<]*(?:(?!<\/({HtmlSanitizeTagBlackList}))<[^<]*)*<\/({HtmlSanitizeTagBlackList})>)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // strip javascript: and unicode representation of javascript:
        // href='javascript:alert(\"gotcha\")'
        // href='&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;:alert(\"gotcha\");'
        static Regex _RegExJavaScriptHref = new Regex(
            @"<.*?(href|src|dynsrc|lowsrc)=.{0,10}((javascript:)|(&#)).*?>",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        static Regex _RegExOnEventAttributes = new Regex(
            @"<.*?\s(on.{4,12}=([""].*?[""]|['].*?['])).*?(>|\/>)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// Sanitizes HTML to some of the most of 
        /// </summary>
        /// <remarks>
        /// This provides rudimentary HTML sanitation catching the most obvious
        /// XSS script attack vectors. For mroe complete HTML Sanitation please look into
        /// a dedicated HTML Sanitizer.
        /// </remarks>
        /// <param name="html">input html</param>
        /// <param name="htmlTagBlacklist">A list of HTML tags that are stripped.</param>
        /// <returns>Sanitized HTML</returns>
        public static string SanitizeHtml(string html, string htmlTagBlacklist = "script|iframe|object|embed|form")
        {
            if (string.IsNullOrEmpty(html))
                return html;

            if (!string.IsNullOrEmpty(htmlTagBlacklist) || htmlTagBlacklist == HtmlSanitizeTagBlackList)
            {
                // Replace Script tags - reused expr is more efficient
                html = _RegExScript.Replace(html, string.Empty);
            }
            else
            {
                html = Regex.Replace(html,
                                        $@"(<({htmlTagBlacklist})\b[^<]*(?:(?!<\/({HtmlSanitizeTagBlackList}))<[^<]*)*<\/({htmlTagBlacklist})>)",
                                        "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            // Remove javascript: directives
            var matches = _RegExJavaScriptHref.Matches(html);
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 2)
                {
                    var txt = match.Value.Replace(match.Groups[2].Value, "unsupported:");
                    html = html.Replace(match.Value, txt);
                }
            }

            // Remove onEvent handlers from elements
            matches = _RegExOnEventAttributes.Matches(html);
            foreach (Match match in matches)
            {
                var txt = match.Value;
                if (match.Groups.Count > 1)
                {
                    var onEvent = match.Groups[1].Value;
                    txt = txt.Replace(onEvent, string.Empty);
                    if (!string.IsNullOrEmpty(txt))
                        html = html.Replace(match.Value, txt);
                }
            }

            return html;
        }

    }
}
