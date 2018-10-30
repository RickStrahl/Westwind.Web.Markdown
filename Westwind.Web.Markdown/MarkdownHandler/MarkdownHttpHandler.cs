using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Westwind.Web.Markdown.Utilities;

namespace Westwind.Web.Markdown
{
    public class MarkdownHttpHandler : IHttpHandler
    {
        /// <summary>
        /// The key used to store the value in the Items Collection
        /// </summary>
        public static string ModelKey = "__MarkdownModel";

        /// <summary>
        /// Configuration settings for the Markdown Handler
        /// </summary>
        public static MarkdownHandlerConfiguration Configuration = new MarkdownHandlerConfiguration();



        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var path = context.Request.PhysicalPath;

            string mdText = File.ReadAllText(path);

            var model = ParseMarkdownToModel(mdText);
            context.Items[ModelKey] = model;

            context.Server.Transfer(Configuration.MarkdownTemplatePagePath);
        }


        public bool IsReusable { get; }


        /// <summary>
        /// Parses the markdown for a title, renders to HTML and returns a model that
        /// contains the relevant items.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private MarkdownModel ParseMarkdownToModel(string markdown, MarkdownModel model = null)
        {
            if (model == null)
                model = new MarkdownModel();


            var firstLines = MarkdownUtils.GetLines(markdown, 30);
            var firstLinesText = String.Join("\n", firstLines);

            // Assume YAML 
            if (markdown.StartsWith("---"))
            {
                var yaml = MarkdownUtils.ExtractString(firstLinesText, "---", "---", returnDelimiters: true);
                if (yaml != null)
                {
                    model.Title = MarkdownUtils.ExtractString(yaml, "title: ", "\n");
                    model.YamlHeader = yaml.Replace("---", "").Trim();
                }
            }

            if (model.Title == null)
            {
                foreach (var line in firstLines.Take(10))
                {
                    if (line.TrimStart().StartsWith("# "))
                    {
                        model.Title = line.TrimStart(new char[] { ' ', '\t', '#' });
                        break;
                    }
                }
            }
        
            model.RawMarkdown = markdown;
            model.RenderedMarkdown = Markdown.ParseHtmlString(markdown, sanitizeHtml: Configuration.SanitizeHtml);

            model.PhysicalPath = HttpContext.Current.Request.PhysicalPath;
            model.RelativePath = HttpContext.Current.Request.Path;

            return model;
        }
    }
   
}
