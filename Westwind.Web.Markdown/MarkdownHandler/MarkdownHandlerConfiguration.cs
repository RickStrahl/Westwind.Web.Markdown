using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Westwind.Web.Markdown
{
    public class MarkdownHandlerConfiguration
    {

        public string MarkdownTemplatePagePath { get; set; } = "~/_MarkdownPage.aspx";

        public bool SanitizeHtml { get; set; } = true;


    }
}
