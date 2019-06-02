using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Factories
{
    public class ClassifierFactory : IClassifierFactory
    {
        protected readonly IServiceProvider Provider;
        protected readonly IIntelligentTaggingSettings Settings;
        protected readonly ISitecoreDataWrapper DataWrapper;

        public ClassifierFactory(IServiceProvider provider, IIntelligentTaggingSettings settings, ISitecoreDataWrapper dataWrapper)
        {
            Provider = provider;
            Settings = settings;
            DataWrapper = dataWrapper;
        }

        public virtual IBaseClassifier Create(string itemId, string database)
        {
            var classifierItem = DataWrapper.GetItemByIdValue(itemId, database);
            var classifier = Create(classifierItem);

            return classifier;
        }

        public virtual IBaseClassifier Create(Item i)
        {
            var classPathField = i.Fields[Settings.ClassPathFieldId];
            var assemblyField = i.Fields[Settings.AssemblyFieldId];

            if (classPathField == null || assemblyField == null)
                return null;
            
            var baseClassifier = Sitecore.Reflection.ReflectionUtil.CreateObject(assemblyField.Value, classPathField.Value, new object[] { Provider, i }) as IBaseClassifier;
            
            return baseClassifier;
        }
    }
}