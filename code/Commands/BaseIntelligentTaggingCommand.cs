using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Factories;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Commands
{
    public class BaseIntelligentTaggingCommand : BaseCommand
    {
        protected readonly IIntelligentTaggingSettings Settings;
        protected readonly IContentService ContentService;
        protected readonly IClassifierFactory ClassifierFactory;

        public BaseIntelligentTaggingCommand()
        {
            Settings = DependencyResolver.Current.GetService<IIntelligentTaggingSettings>();
            ContentService = DependencyResolver.Current.GetService<IContentService>();
            ClassifierFactory = DependencyResolver.Current.GetService<IClassifierFactory>();
        }
    }
}