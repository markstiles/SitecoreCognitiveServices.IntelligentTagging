using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Services
{
    public interface ISetupService
    { 
        void SaveKeysAndTest(string naturalLanguageClassifierEndpoint, string naturalLanguageClassifierUsername, string naturalLanguageClassifierPassword);
        void UpdateKey(ID fieldId, string value);
        void PublishContent();
        void ResetDictionary();
    }
}