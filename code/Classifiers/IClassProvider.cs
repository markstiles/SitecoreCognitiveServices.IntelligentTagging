using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers
{
    public interface IClassProvider
    {
        List<string> Classify(Item itemToTag, string text);
    }
}