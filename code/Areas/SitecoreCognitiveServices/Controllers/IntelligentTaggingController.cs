using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Sitecore;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Jobs;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Setup;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.ViewModels;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Factories;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Services;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Statics;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.IBMSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Controllers {

    public class IntelligentTaggingController : Controller
    {
        #region Constructor

        protected readonly IWebUtilWrapper WebUtil;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentTaggingSettings Settings;
        protected readonly ISetupInformationFactory SetupFactory;
        protected readonly ISetupService SetupService;
        protected readonly IContentSearchWrapper ContentSearch;
        protected readonly INaturalLanguageClassifierService NaturalLanguageClassifier;
        protected readonly IContentService ContentService;
        protected readonly IContentSearchService ContentSearchService;
        protected readonly IClassifierFactory ClassifierFactory;

        protected string Id;
        protected string Language;
        protected string Database;

        public IntelligentTaggingController(
            IWebUtilWrapper webUtil,
            ISitecoreDataWrapper dataWrapper,
            IIntelligentTaggingSettings settings,
            ISetupInformationFactory setupFactory,
            ISetupService setupService,
            IContentSearchWrapper contentSearch,
            INaturalLanguageClassifierService naturalLanguageClassifier,
            IContentService contentService,
            IContentSearchService contentSearchService,
            IClassifierFactory classifierFactory)
        {
            WebUtil = webUtil;
            DataWrapper = dataWrapper;
            Settings = settings;
            SetupFactory = setupFactory;
            SetupService = setupService;
            ContentSearch = contentSearch;
            NaturalLanguageClassifier = naturalLanguageClassifier;
            ContentService = contentService;
            ContentSearchService = contentSearchService;
            ClassifierFactory = classifierFactory;

            Id = WebUtil.GetQueryString("id");
            Language = WebUtil.GetQueryString("language");
            Database = WebUtil.GetQueryString("db");
        }

        #endregion

        #region Train Classifier

        public ActionResult TrainClassifier(string id, string language, string db)
        {
            // check for any missing field values

            return View("TrainClassifier");
        }

        [HttpPost]
        public ActionResult TrainClassifierPost(string id, string database)
        {
            if (!IsSitecoreUser()) return LoginPage();
            
            var classifier = ClassifierFactory.Create(id, database);
            var classTrainer = classifier as IClassTrainer;
            if (classTrainer == null)
                return Json(new { Failed = true, Message = Translator.Text("TrainClassifier.NotAClassTrainer") });
            
            var dataResult = classTrainer.GetTrainingData();
            if(dataResult.Item1 == false)
                return Json(new { Failed = true, Message = Translator.Text("TrainClassifier.GetTrainingDataFailed") });

            var trainResult = classTrainer.TrainModel(Request, dataResult.Item2);

            return trainResult.Item1 == false
                ? Json(new { Failed = true, Message = Translator.Text("TestClassifier.TrainingFailed") })
                : Json(new { Failed = false, Message = "" });
        }

        #endregion

        #region Test Classifier

        public ActionResult TestClassifier(string id, string language, string db)
        {
            if (!IsSitecoreUser()) return LoginPage();

            var viewName = "TestClassifier";
            var classifier = ClassifierFactory.Create(id, db);
            var classTrainer = classifier as IClassTrainer;
            if (classTrainer == null)
                return View(viewName, new TestClassifierViewModel(true, Translator.Text("TestClassifier.NotAClassTrainer")));
            
            var status = classTrainer.GetTrainingStatus();

            return View(viewName, new TestClassifierViewModel(!status.Item1, status.Item2));
        }

        [HttpPost]
        public ActionResult TestClassifierPost(string id, string language, string database)
        {
            var classifier = ClassifierFactory.Create(id, database);
            var classTrainer = classifier as IClassTrainer;
            if(classTrainer == null)
                return Json(new { Failed = true, Message = Translator.Text("TestClassifier.NotAClassTrainer") });

            var result = classTrainer.TestClassifier();
            
            return Json(new { Failed = !result.Item1, Message = result.Item2 });
        }

        #endregion

        #region Learn Type

        public ActionResult LearnType()
        {
            if (!IsSitecoreUser()) return LoginPage();

            var viewModel = new LearnTypeViewModel();
            Item currentItem = DataWrapper.GetItemByIdValue(Id, Database);
            if (currentItem != null) {
                currentItem.Fields.ReadAll();
                viewModel.Fields.AddRange(currentItem.Fields.Select(a => a.Name).Where(n => !n.StartsWith("__")).OrderBy(a => a));
            }

            return View("LearnType", viewModel);
        }

        [HttpPost]
        public ActionResult LearnTypePost(string id, string language, string db, List<string> contentFields)
        {
            if (!IsSitecoreUser()) return LoginPage();

            if (contentFields.Contains(""))
                return Json(new { Failed = true, Message = "No Content Fields were selected." });

            Item currentItem = DataWrapper.GetItemByIdValue(id, db);
            if(currentItem == null)
                return Json(new { Failed = true, Message = $"Item Id:{id} or Datbase:{db} is not correct." });
            
            foreach (var f in contentFields)
            {
                if (currentItem.Fields[f] == null)
                    return Json(new { Failed = true, Message = $"Content Field:{f} is not a field on this item." });
            }
            
            Item newMapping = ContentService.AddMapping(db, 
                currentItem.TemplateName, 
                contentFields.Select(a => currentItem.Fields[a].ID.ToString()).ToList(), 
                currentItem.TemplateID.ToString());
            if(newMapping == null)
                return Json(new { Failed = true, Message = "Item type was not learned." });
            
            return Json(new { Failed = false });
        }
        
        #endregion

        #region Tag Content

        public ActionResult TagContent()
        {
            if (!IsSitecoreUser()) return LoginPage();

            var currentItem = DataWrapper.GetItemByIdValue(Id, Database);
            var classifiers = ContentService.GetClassifiers(Database)
                .Where(a => a.SupportsThisItem(currentItem))
                .ToList();
            if (!classifiers.Any())
                return View("TagContent");

            var templateId = currentItem.TemplateID.Guid;
            var map = ContentService.GetContentMappings(Database)[templateId];
            var content = ContentService.GetContent(currentItem, map.ContentFields);
            var classifierListItems = classifiers
                .Select(a => new ListItem(a.Name, a.InnerItem.ID.ToString()))
                .ToList();
            var viewModel = new TagContentViewModel(content, classifierListItems);

            return View("TagContent", viewModel);
        }

        [HttpPost]
        public ActionResult TagContentPost(string id, string database, string classifierId, string content)
        {
            if (!IsSitecoreUser()) return LoginPage();

            var currentItem = DataWrapper.GetItemByIdValue(id, database);

            var classifierItem = DataWrapper.GetItemByIdValue(classifierId, database);
            var classifier = ClassifierFactory.Create(classifierItem);

            var classProvider = classifier as IClassProvider;
            if(classProvider == null)
                return Json(new { Failed = true, Tags = "" });

            if (content.Length > 1022)
                content = content.Substring(0, 1022);
            
            var tags = classProvider.Classify(currentItem, content);
            if (tags == null || !tags.Any())
                return Json(new { Failed = true, Tags = "" });

            return Json(new
            {
                Failed = false,
                Tags = tags.Select(a => a).ToList()
            });
        }

        [HttpPost]
        public ActionResult SaveTagsToItem(string id, string language, string database, List<string> tags, string classifierId)
        {
            if (!IsSitecoreUser()) return LoginPage();

            var classifier = ContentService.GetClassifiers(database).First(a => a.InnerItem.ID.ToString().Equals(classifierId));
            if (classifier == null)
                return Json(new { Failed = true, Message = "Classifier was null." });

            var ctxItem = DataWrapper.GetItemByIdValue(id, database);
            if (ctxItem == null)
                return Json(new { Failed = true, Message = "Context item was null." });
            
            var tagIds = ContentSearchService
                .GetTags(classifier.TaxonomyFolderId, language, database, tags)
                .Select(a => a.ItemId);

            DataWrapper.UpdateFields(ctxItem, new Dictionary<ID, string>
            {
                { classifier.DestinationTagsFieldId, string.Join("|", tagIds) }  
            });

            return Json(new { Failed = false });
        }

        #endregion
        
        #region Setup

        public ActionResult Setup()
        {
            if (!IsSitecoreUser()) return LoginPage();
            
            var db = Sitecore.Configuration.Factory.GetDatabase(Settings.MasterDatabase);
            using (new DatabaseSwitcher(db))
            {
                ISetupInformation info = SetupFactory.Create();

                return View("Setup", info);
            }
        }

        public ActionResult SetupSubmit(string naturalLanguageClassifierEndpoint, string naturalLanguageClassifierUsername, string naturalLanguageClassifierPassword)
        {
            if (!IsSitecoreUser()) return LoginPage();

            SetupService.SaveKeysAndTest(naturalLanguageClassifierEndpoint, naturalLanguageClassifierUsername, naturalLanguageClassifierPassword);
            SetupService.PublishContent();
            SetupService.ResetDictionary();

            return Json(new
            {
                Failed = false,
                Items = ""
            });
        }

        #endregion

        #region Shared

        public ActionResult GetJobStatus(string handleName)
        {
            Job j = JobManager.GetJob(handleName);

            var message = j != null && j.Status.Messages.Count > 0
                ? j.Status.Messages[j.Status.Messages.Count - 1]
                : "";

            return Json(new
            {
                Current = j?.Status.Processed ?? 0,
                Total = j?.Status.Total ?? 0,
                Completed = j?.IsDone ?? true,
                Message = message
            });
        }

        public bool IsSitecoreUser()
        {
            return DataWrapper.ContextUser.IsAuthenticated
                   && DataWrapper.ContextUser.Domain.Name.ToLower().Equals("sitecore");
        }

        public ActionResult LoginPage()
        {
            return new RedirectResult("/sitecore/login");
        }

        #endregion
    }
}