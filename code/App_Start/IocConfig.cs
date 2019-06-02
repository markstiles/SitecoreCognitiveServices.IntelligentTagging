using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Controllers;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Setup;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Factories;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.App_Start
{
    public class IocConfig : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            //settings
            serviceCollection.AddTransient<IIntelligentTaggingSettings, IntelligentTaggingSettings>();
            
            //models
            serviceCollection.AddTransient<ISetupInformation, SetupInformation>();
            serviceCollection.AddTransient<IContentMapping, ContentMapping>();
            serviceCollection.AddTransient<IBaseClassifier, BaseClassifier>();

            //factories
            serviceCollection.AddTransient<ISetupInformationFactory, SetupInformationFactory>();
            serviceCollection.AddTransient<IContentMappingFactory, ContentMappingFactory>();
            serviceCollection.AddTransient<IClassifierFactory, ClassifierFactory>();

            //services
            serviceCollection.AddTransient<ISetupService, SetupService>();
            serviceCollection.AddTransient<IContentService, ContentService>();
            serviceCollection.AddTransient<IContentSearchService, ContentSearchService>();

            //controller
            serviceCollection.AddTransient(typeof(IntelligentTaggingController));
        }
    }
}