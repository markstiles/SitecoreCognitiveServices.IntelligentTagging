using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data;
using Sitecore.Data.Fields;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Factories
{
    public class ContentMappingFactory : IContentMappingFactory
    {
        protected readonly IServiceProvider Provider;
        protected readonly IIntelligentTaggingSettings Settings;

        public ContentMappingFactory(IServiceProvider provider, IIntelligentTaggingSettings settings)
        {
            Provider = provider;
            Settings = settings;
        }

        public virtual IContentMapping Create(Item i)
        {
            var obj = Provider.GetService<IContentMapping>();
            
            var f1 = i.Fields[Settings.TemplateFieldId] != null ? i[Settings.TemplateFieldId] : "";
            Guid g1;
            if (!Guid.TryParse(f1, out g1))
                return null;

            var f2 = i.Fields[Settings.ContentFieldsId] != null ? ((DelimitedField)i.Fields[Settings.ContentFieldsId]).Items : new string[0];
            var list = new List<ID>();
            foreach (var t in f2)
            {
                Guid g2;
                if (!Guid.TryParse(t, out g2))
                    return null;

                list.Add(new ID(g2));
            }

            obj.InnerItem = i;
            obj.TemplateField = new ID(g1);
            obj.ContentFields = list;
            
            return obj;
        }
    }
}