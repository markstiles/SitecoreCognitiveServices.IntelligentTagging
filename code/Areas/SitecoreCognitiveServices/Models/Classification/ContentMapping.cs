using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification
{
    public class ContentMapping : IContentMapping
    {
        public List<ID> ContentFields { get; set; }
        public ID TemplateField { get; set; }
        public Item InnerItem { get; set; }
    }
}