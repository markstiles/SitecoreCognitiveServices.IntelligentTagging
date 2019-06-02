using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging
{
    public interface IIntelligentTaggingSettings
    {
        string WebDatabase { get; }
        string MasterDatabase { get; }
        string CoreDatabase { get; }
        ID EnumerationNameFieldId { get; }
        ID EnumerationValueFieldId { get; }
        ID SCSDKTemplatesFolderId { get; }
        ID SCSModulesFolderId { get; }
        ID ContentNodeId { get; }
        ID TemplatesFolderId { get; }
        ID IntelligentContentFolderId { get; }
        ID ContentMappingsFolderId { get; }
        ID ContentMappingTemplateId { get; }
        ID ClassifierTemplateId { get; }
        ID ClassifiersFolderId { get; }
        ID LanguagesFolderId { get; }
        ID SourceTagsFieldId { get; }
        ID DestinationTagsFieldId { get; }
        ID ContentFieldsId { get; }
        ID TemplateFieldId { get; }
        string DictionaryDomain { get; }
        ID ClassifierIdFieldId { get; }
        ID ClassifierNameFieldId { get; }
        ID ClassifierLanguageFieldId { get; }
        ID ClassifierCreatedFieldId { get; }
        ID LearnedTypesFieldId { get; }
        ID ItemTrainingCountFieldId { get; }
        ID ItemTestingCountFieldId { get; }
        ID AccuracyFieldId { get; }
        ID OverageFieldId { get; }
        ID TaxonomyFieldId { get; }
        ID TaxonomyItemTemplateFieldId { get; }
        ID ConfidenceFieldId { get; }
        ID ClassPathFieldId { get; }
        ID AssemblyFieldId { get; }
        ID RulesFieldId { get; }
        ID TrainingDataFieldId { get; }
        bool HasNoValue(string str);
        bool MissingKeys();
    }
}
