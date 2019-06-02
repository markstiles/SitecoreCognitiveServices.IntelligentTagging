using System;
using Microsoft.Extensions.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Setup;
using SitecoreCognitiveServices.Foundation.IBMSDK;
using SitecoreCognitiveServices.Foundation.IBMSDK.Http;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Factories
{
    public class SetupInformationFactory : ISetupInformationFactory
    {
        protected readonly IServiceProvider Provider;
        protected readonly IIBMWatsonApiKeys ApiKeys;

        public SetupInformationFactory(IServiceProvider provider, IIBMWatsonApiKeys apiKeys)
        {
            Provider = provider;
            ApiKeys = apiKeys;
        }

        public virtual ISetupInformation Create()
        {
            var obj = Provider.GetService<ISetupInformation>();
            obj.NaturalLanguageClassifierEndpoint = ApiKeys.NaturalLanguageClassifierEndpoint;
            obj.NaturalLanguageClassifierUsername = ApiKeys.NaturalLanguageClassifierUsername;
            obj.NaturalLanguageClassifierPassword = ApiKeys.NaturalLanguageClassifierPassword;
            
            return obj;
        }
    }
}