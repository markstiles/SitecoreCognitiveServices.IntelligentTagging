using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Services;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Statics;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.IBMSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers
{
    public class WatsonClassifier : BaseClassifier, IClassTrainer, IClassProvider
    {
        #region Constructor

        public string ClassifierId { get; set; }
        public ID Language { get; set; }
        public string LanguageCode { get; set; }
        public DateTime Created { get; set; }
        public List<IContentMapping> ContentMappings { get; set; }
        public int ItemTrainingCount { get; set; }
        public int ItemTestingCount { get; set; }
        public string TrainingData { get; set; }

        protected readonly IContentSearchWrapper ContentSearch;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IContentService ContentService;
        protected readonly IContentSearchService ContentSearchService;
        protected readonly INaturalLanguageClassifierService NaturalLanguageClassifier;

        public WatsonClassifier(IServiceProvider provider, Item classifierItem) : base(provider, classifierItem)
        {
            ContentSearch = provider.GetService<IContentSearchWrapper>();
            DataWrapper = provider.GetService<ISitecoreDataWrapper>();
            ContentService = provider.GetService<IContentService>();
            ContentSearchService = provider.GetService<IContentSearchService>();
            NaturalLanguageClassifier = provider.GetService<INaturalLanguageClassifierService>();

            ClassifierId = GetString(classifierItem, Settings.ClassifierIdFieldId);
            Language = GetID(classifierItem, Settings.ClassifierLanguageFieldId);
            var languageItem = classifierItem.Database.GetItem(Language);
            LanguageCode = GetString(languageItem, Settings.EnumerationValueFieldId);
            Created = GetDate(classifierItem, Settings.ClassifierCreatedFieldId); var typeItems = GetDelimitedItems(classifierItem, Settings.LearnedTypesFieldId);
            ContentMappings = typeItems
                .Select(a => Mappingfactory.Create(a))
                .Where(b => b != null)
                .ToList();            
            ItemTrainingCount =  GetInt(classifierItem, Settings.ItemTrainingCountFieldId);
            ItemTestingCount = GetInt(classifierItem, Settings.ItemTestingCountFieldId);
            TrainingData = GetString(classifierItem, Settings.TrainingDataFieldId);
        }

        #endregion

        #region Base Classifier

        public override bool SupportsThisItem(Item itemToTag)
        {
            return ContentMappings.Any(c => c.TemplateField.Guid == itemToTag.TemplateID.Guid);
        }

        #endregion

        #region IClassTrainer

        public Tuple<bool, string> GetTrainingData()
        {
            //get tags from taxonomy mapping folder field
            var tagItems = ContentSearchService.GetTagsByTemplate(TaxonomyFolderId, LanguageCode, InnerItem.Database.Name, TaxonomyItemTemplateIds);
            var results = new Dictionary<Guid, string>();
            var trainingData = new List<string>();
            foreach (var tag in tagItems)
            {
                var contentTypes = ContentMappings
                    .Select(a => a.TemplateField.Guid).ToDictionary(a => a);

                //show on images that have no face item pointing to them
                var links = Globals.LinkDatabase.GetItemReferrers(tag.GetItem(), false);
                var contentItems = links
                    .Select(a => a.GetSourceItem())
                    .Where(a => contentTypes.ContainsKey(a.TemplateID.Guid))
                    .Take(ItemTrainingCount)
                    .ToList();

                //todo: if there aren't enough content items for any given tag, we could log that and mention it as an issue for training
                foreach (var c in contentItems)
                {
                    //dedupe
                    if (results.ContainsKey(c.ID.Guid))
                        continue;

                    results.Add(c.ID.Guid, string.Empty);

                    //pull content for the items
                    var contentMap = ContentMappings.First(a => a.TemplateField.Guid == c.TemplateID.Guid);
                    var trainingRow = ContentService.GetTrainingData(c, SourceTagsFieldId, contentMap.ContentFields);
                    if (string.IsNullOrWhiteSpace(trainingRow))
                        continue;

                    trainingData.Add(trainingRow);
                }
            }

            if (trainingData.Count == 0)
                return Tuple.Create(false, "There was no training data");

            var trainingDataString = string.Join(Environment.NewLine, trainingData);
            DataWrapper.UpdateFields(InnerItem, new Dictionary<ID, string>
            {
                { Settings.TrainingDataFieldId, trainingDataString }
            });

            //var range = ItemTrainingCount * tagItems.Count;
            //if (trainingData.Count < range)
            //    return Tuple.Create(false, $"There's only {trainingData.Count} of the required {range} items. {ItemTrainingCount} items per tag (change in settings).");
            
            return Tuple.Create(true, trainingDataString);
        }

        public Tuple<bool, string> TrainModel(HttpRequestBase request, string trainingData)
        {
            //build file
            var relativePath = $"temp\\classifier-{Name}-{DateTime.UtcNow:yyyy-MM-dd-mm-ss}.csv";
            var filePath = $"{request.PhysicalApplicationPath}{relativePath}";
            System.IO.File.WriteAllText(filePath, trainingData);
            var urlPath = $"{request.Url.GetLeftPart(UriPartial.Authority)}/{relativePath}";
            
            //make training request - (currently fails due to the size of the training set being posted)
            var apiClassifier = NaturalLanguageClassifier.CreateClassifier(Name, LanguageCode, trainingData);
            if (apiClassifier == null)
                return Tuple.Create(false, $"The API could accept the training data. You can upload it manually by downloading it here: {urlPath}");

            DataWrapper.UpdateFields(InnerItem, new Dictionary<ID, string>
            {
                { Settings.ClassifierIdFieldId, apiClassifier.classifier_id },
                { Settings.ClassifierCreatedFieldId, DateUtil.ToIsoDate(apiClassifier.created) }
            });

            return Tuple.Create(true, "");
        }

        public Tuple<bool, string> GetTrainingStatus()
        {
            //check if it's still training
            var classifierInfo = NaturalLanguageClassifier.GetClassifierInfo(ClassifierId);
            if (classifierInfo?.status == null)
                return Tuple.Create(false, Translator.Text("TestClassifier.ClassifierIsNull"));
            
            //Non Existent, Training, Failed, Available, Unavailable
            if (classifierInfo.status == "Non Existent")
                return Tuple.Create(false, Translator.Text("TestClassifier.ClassifierDoesNotExist"));
            if (classifierInfo.status == "Failed")
                return Tuple.Create(false, Translator.Text("TestClassifier.ClassifierTrainingFailed"));
            if (classifierInfo.status == "Unavailable")
                return Tuple.Create(false, Translator.Text("TestClassifier.ClassifierIsUnavailable"));
            if (classifierInfo.status == "Training")
                return Tuple.Create(false, Translator.Text("TestClassifier.ClassifierIsTraining"));

            return Tuple.Create(true, "");
        }

        public Tuple<bool, string> TestClassifier()
        {
            var database = InnerItem.Database.Name;
            
            //start testing content for accuracy
            var testItems = ContentSearchService.GetContent(database, LanguageCode, ContentMappings.Select(a => a.TemplateField).ToList(), ItemTestingCount);
            if (testItems.Count < ItemTestingCount)
                return Tuple.Create(false, $"There's only {testItems.Count} of the required {ItemTestingCount} items (change in settings).");

            var mappings = ContentService.GetContentMappings(database);
            var accuracySum = 0f;
            var overageSum = 0f;
            var confidenceSum = 0f;
            var counted = 0;
            foreach (var t in testItems)
            {
                if (!mappings.ContainsKey(t.TemplateId.Guid))
                    continue;

                var map = mappings[t.TemplateId.Guid];
                Item contentItem = DataWrapper.GetItemById(t.ItemId, database);
                if (contentItem == null)
                    continue;

                var testingTags = ContentService.GetTags(contentItem, SourceTagsFieldId);
                var testingContent = ContentService.GetTrimmedContent(contentItem, map.ContentFields);
                if (!testingTags.Any() || string.IsNullOrWhiteSpace(testingContent))
                    continue;

                var tags = NaturalLanguageClassifier.Classify(ClassifierId, testingContent)?.classes;
                if (tags == null || !tags.Any())
                    continue;

                var suggestedTagNames = tags.Select(a => a.class_name).ToList();
                var confidenceScores = tags.Where(a => testingTags.Contains(a.class_name)).Select(b => b.confidence);
                var tagsMatched = testingTags.Count(a => suggestedTagNames.Contains(a));
                var tagAccuracy = (float)tagsMatched / testingTags.Count;
                accuracySum += tagAccuracy;
                overageSum += (suggestedTagNames.Count > testingTags.Count) ? suggestedTagNames.Count - testingTags.Count : 0;
                confidenceSum += confidenceScores.Average();
                counted++;
            }

            var accuracy = (float)accuracySum / counted;
            var accuracyStr = $"{accuracy * 100:F0}%";

            var overage = (float)overageSum / counted;
            var overageStr = $"{overage:F2}";

            var confidence = (float)confidenceSum / counted;
            var confidenceStr = $"{confidence * 100:F0}%";

            DataWrapper.UpdateFields(InnerItem, new Dictionary<ID, string>
            {
                { Settings.AccuracyFieldId, accuracyStr },
                { Settings.OverageFieldId, overageStr },
                { Settings.ConfidenceFieldId, confidenceStr }
            });

            return Tuple.Create(true, "");
        }

        #endregion

        #region IClassProvider

        public List<string> Classify(Item itemToTag, string text)
        {
            if(string.IsNullOrWhiteSpace(text))
                return new List<string>();

            if (text.Length > 1022)
                text = text.Substring(0, 1022);
            
            var tags = NaturalLanguageClassifier
                .Classify(ClassifierId, text)?
                .classes
                .Where(b => b.confidence > 0.01)
                .Select(a => a.class_name)
                .ToList();

            return tags;
        }
        
        #endregion
    }
}