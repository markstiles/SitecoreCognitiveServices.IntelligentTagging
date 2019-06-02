using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers
{
    public class BaseClassifier : IBaseClassifier
    {
        #region Constructor 

        public Item InnerItem { get; set; }
        public string Name { get; set; }
        public ID TaxonomyFolderId { get; set; }
        public List<ID> TaxonomyItemTemplateIds { get; set; }
        public ID SourceTagsFieldId { get; set; }
        public ID DestinationTagsFieldId { get; set; }

        protected readonly IIntelligentTaggingSettings Settings;
        protected readonly IContentMappingFactory Mappingfactory;

        public BaseClassifier(IServiceProvider provider, Item classifierItem)
        {
            Settings = provider.GetService<IIntelligentTaggingSettings>();
            Mappingfactory = provider.GetService<IContentMappingFactory>();

            InnerItem = classifierItem;
            Name = GetString(classifierItem, Settings.ClassifierNameFieldId);
            TaxonomyFolderId = GetID(classifierItem, Settings.TaxonomyFieldId);
            TaxonomyItemTemplateIds = GetDelimitedIDs(classifierItem, Settings.TaxonomyItemTemplateFieldId);
            SourceTagsFieldId = GetID(classifierItem, Settings.SourceTagsFieldId);
            DestinationTagsFieldId = GetID(classifierItem, Settings.DestinationTagsFieldId);
        }

        #endregion

        public virtual bool SupportsThisItem(Item itemToTag)
        {
            return true;
        }

        #region Field Helpers

        public string GetString(Item item, ID fieldId)
        {
            if (item.Fields[fieldId] == null)
                return string.Empty;

            var fieldValue = item[fieldId];

            return fieldValue;
        }

        public int GetInt(Item item, ID fieldId)
        {
            if (item.Fields[fieldId] == null)
                return 0;

            var fieldValue = item[fieldId];
            if (string.IsNullOrWhiteSpace(fieldValue))
                return 0;


            return int.Parse(fieldValue);
        }

        public DateTime GetDate(Item item, ID fieldId)
        {
            if (item.Fields[fieldId] == null)
                return DateTime.MinValue;

            var fieldValue = ((DateField)item.Fields[fieldId]).DateTime;

            return fieldValue;
        }

        public List<ID> GetDelimitedIDs(Item item, ID fieldId)
        {
            var types = new List<ID>();
            if (item.Fields[fieldId] == null)
                return types;

            var fieldList = ((DelimitedField)item.Fields[fieldId]).Items;
            foreach (var t in fieldList)
            {
                Guid guid;
                if (!Guid.TryParse(t, out guid))
                    continue;

                types.Add(new ID(guid));
            }

            return types;
        }

        public List<Item> GetDelimitedItems(Item item, ID fieldId)
        {
            var ids = GetDelimitedIDs(item, fieldId);
            var items = ids.Select(a => item.Database.GetItem(a)).Where(b => b != null).ToList();

            return items;
        }

        public ID GetID(Item item, ID fieldId)
        {
            if (item.Fields[fieldId] == null)
                return ID.Null;

            var fieldValue = item[fieldId];
            Guid guid;
            if (!Guid.TryParse(fieldValue, out guid))
                return ID.Null;

            return new ID(guid);
        }

        #endregion
    }
}