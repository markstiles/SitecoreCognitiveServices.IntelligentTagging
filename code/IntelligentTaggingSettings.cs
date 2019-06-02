using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using SitecoreCognitiveServices.Foundation.IBMSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging
{
    public class IntelligentTaggingSettings : IIntelligentTaggingSettings
    {
        protected readonly IIBMWatsonApiKeys IBMApiKeys;
        protected readonly ISitecoreDataWrapper DataWrapper;

        public IntelligentTaggingSettings(
            ISitecoreDataWrapper dataWrapper,
            IIBMWatsonApiKeys ibmApiKeys)
        {
            IBMApiKeys = ibmApiKeys;
            DataWrapper = dataWrapper;
        }

        #region Globally Shared Settings

        public virtual string CoreDatabase => Settings.GetSetting("CognitiveService.CoreDatabase");
        public virtual string MasterDatabase => Settings.GetSetting("CognitiveService.MasterDatabase");
        public virtual string WebDatabase => Settings.GetSetting("CognitiveService.WebDatabase");
        public virtual ID EnumerationNameFieldId => new ID(Settings.GetSetting("CognitiveService.EnumerationNameFieldId"));
        public virtual ID EnumerationValueFieldId => new ID(Settings.GetSetting("CognitiveService.EnumerationValueFieldId"));
        public virtual ID SCSDKTemplatesFolderId => new ID(Settings.GetSetting("CognitiveService.SCSDKTemplatesFolder"));
        public virtual ID SCSModulesFolderId => new ID(Settings.GetSetting("CognitiveService.SCSModulesFolder"));
        public virtual ID ContentNodeId => new ID(Settings.GetSetting("CognitiveService.ContentNode"));

        #endregion

        public virtual ID TemplatesFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.TemplatesFolderId"));
        public virtual ID IntelligentContentFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.IntelligentContentFolderId"));
        public virtual ID ContentMappingsFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ContentMappingsFolderId"));
        public virtual ID ContentMappingTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ContentMappingTemplateId"));
        public virtual ID ClassifierTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ClassifierTemplateId"));
        public virtual ID ClassifiersFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ClassifiersFolderId"));
        public virtual ID LanguagesFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.LanguagesFolderId"));
        
        public virtual ID SourceTagsFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.SourceTagsFieldId"));
        public virtual ID DestinationTagsFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.DestinationTagsFieldId"));
        public virtual ID ContentFieldsId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ContentFieldsId"));
        public virtual ID TemplateFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.TemplateFieldId"));
        public virtual string DictionaryDomain => Settings.GetSetting("CognitiveService.IntelligentTagging.DictionaryDomain");
        public virtual ID ClassifierIdFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ClassifierIdFieldId"));
        public virtual ID ClassifierNameFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ClassifierNameFieldId"));
        public virtual ID ClassifierLanguageFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ClassifierLanguageFieldId"));
        public virtual ID ClassifierCreatedFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ClassifierCreatedFieldId"));
        public virtual ID LearnedTypesFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.LearnedTypesFieldId"));
        public virtual ID ItemTrainingCountFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ItemTrainingCountFieldId"));
        public virtual ID ItemTestingCountFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ItemTestingCountFieldId"));
        public virtual ID AccuracyFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.AccuracyFieldId"));
        public virtual ID OverageFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.OverageFieldId"));
        public virtual ID ConfidenceFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ConfidenceFieldId"));
        public virtual ID TaxonomyFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.TaxonomyFieldId"));
        public virtual ID TaxonomyItemTemplateFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.TaxonomyItemTemplateFieldId"));
        public virtual ID ClassPathFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.ClassPathFieldId"));
        public virtual ID AssemblyFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.AssemblyFieldId"));
        public virtual ID RulesFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.RulesFieldId"));
        public virtual ID TrainingDataFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentTagging.TrainingDataFieldId"));

        public bool MissingKeys()
        {
            if (IBMApiKeys == null)
                return true;

            return HasNoValue(IBMApiKeys.NaturalLanguageClassifierEndpoint)
                   || HasNoValue(IBMApiKeys.NaturalLanguageClassifierUsername)
                   || HasNoValue(IBMApiKeys.NaturalLanguageClassifierPassword);
        }
        public bool HasNoValue(string str)
        {
            return string.IsNullOrWhiteSpace(str.Trim());
        }
    }
}