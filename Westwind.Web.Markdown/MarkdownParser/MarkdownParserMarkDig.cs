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

using System;
using System.IO;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;

namespace Westwind.Web.Markdown.MarkdownParser
{

    /// <summary>
    /// Wrapper around the CommonMark.NET parser that provides a cached
    /// instance of the Markdown parser. Hooks up custom processing.
    /// </summary>
    public class  MarkdownParserMarkdig : MarkdownParserBase
    {
        public static MarkdownPipeline Pipeline;

        private readonly bool _usePragmaLines;

        public MarkdownParserMarkdig(bool usePragmaLines = false, bool force = false)
        {
            _usePragmaLines = usePragmaLines;
            if (force || Pipeline == null)
            {
                var builder = CreatePipelineBuilder();
                Pipeline = builder.Build();
            }
        }

        /// <summary>
        /// Parses the actual markdown down to html
        /// </summary>
        /// <param name="markdown">Markdown to parse</param>
        /// <param name="sanitizeHtml">Sanitizes generated HTML by stripping scriptable html markup and directives</param>
        /// <returns></returns>        
        public override string Parse(string markdown, bool sanitizeHtml = true)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            var htmlWriter = new StringWriter();
            var renderer = CreateRenderer(htmlWriter);

            Markdig.Markdown.Convert(markdown, renderer, Pipeline);

            var html = htmlWriter.ToString();
            
            html = ParseFontAwesomeIcons(html);

            //if (!mmApp.Configuration.MarkdownOptions.AllowRenderScriptTags)
            if (sanitizeHtml)
                html = SanitizeHtml(html);  
                      
            return html;
        }

        /// <summary>
        /// Allow interception of the Markdown Pipeline creation for Markdig
        /// Implement this func that returns a MarkdownPipelineBuilder that's
        /// configured for your preferences.
        ///
        /// bool usePragmaLines  - determines whether line numbers are generated
        /// </summary>
        public static Func<bool, MarkdownPipelineBuilder> OnCreateMarkdigPipeline;


        protected virtual MarkdownPipelineBuilder CreatePipelineBuilder()
        {
        
            if (OnCreateMarkdigPipeline != null)
                return OnCreateMarkdigPipeline(_usePragmaLines);

            var builder = new MarkdownPipelineBuilder()
                .UseEmphasisExtras()
                .UsePipeTables()
                .UseGridTables()
                .UseFooters()
                .UseFootnotes()
                .UseCitations()
                .UseAutoLinks() // URLs are parsed into anchors
                .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                .UseAbbreviations()
                .UseYamlFrontMatter()
                .UseEmojiAndSmiley(true)
                .UseMediaLinks()
                .UseListExtras()
                .UseFigures()
                .UseTaskLists()
                .UseCustomContainers()
                .UseGenericAttributes();


            if (_usePragmaLines)
                builder = builder.UsePragmaLines();

            return builder;
        }

        protected virtual IMarkdownRenderer CreateRenderer(TextWriter writer)
        {
            return new HtmlRenderer(writer);
        }
    }
}
