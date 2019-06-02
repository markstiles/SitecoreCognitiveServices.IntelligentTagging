using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Factories
{
    public interface IClassifierFactory
    {
        IBaseClassifier Create(string itemId, string database);
        IBaseClassifier Create(Item i);
    }
}