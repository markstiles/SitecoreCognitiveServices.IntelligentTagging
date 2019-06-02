using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers
{
    public interface IBaseClassifier
    {
        Item InnerItem { get; set; }
        string Name { get; set; }
        ID TaxonomyFolderId { get; set; }
        List<ID> TaxonomyItemTemplateIds { get; set; }
        ID SourceTagsFieldId { get; set; }
        ID DestinationTagsFieldId { get; set; }

        bool SupportsThisItem(Item itemToTag);
        string GetString(Item item, ID fieldId);
        int GetInt(Item item, ID fieldId);
        DateTime GetDate(Item item, ID fieldId);
        List<ID> GetDelimitedIDs(Item item, ID fieldId);
        List<Item> GetDelimitedItems(Item item, ID fieldId);
        ID GetID(Item item, ID fieldId);
    }
}