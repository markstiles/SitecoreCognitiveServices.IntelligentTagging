using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification
{
    public interface IContentMapping
    {
        List<ID> ContentFields { get; set; }
        ID TemplateField { get; set; }
        Item InnerItem { get; set; }
    }
}