using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Services
{
    public interface IContentService
    {
        List<IBaseClassifier> GetClassifiers(string database);
        Dictionary<Guid, IContentMapping> GetContentMappings(string database);
        Item AddMapping(string database, string newItemTemplateName, List<string> contentFieldIds, string templateId);
        string GetTrainingData(Item contentItem, ID sourceTagsField, List<ID> contentFieldIds);
        List<string> GetTags(Item item, ID sourceTagsField);
        string GetContent(Item item, List<ID> contentFieldIds);
        string GetTrimmedContent(Item item, List<ID> contentFieldIds);
        string StripInvalidChars(string htmlString);
    }
}