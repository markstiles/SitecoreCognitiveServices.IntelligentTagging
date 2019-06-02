using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using SitecoreCognitiveServices.Foundation.IBMSDK;
using SitecoreCognitiveServices.Foundation.SCSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Services
{
    public class SetupService : ISetupService
    {
        #region Constructor

        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIBMWatsonApiKeys IBMApiKeys;
        protected readonly IIntelligentTaggingSettings Settings;
        protected readonly IPublishWrapper PublishWrapper;
        protected readonly ISCSDKSettings SCSDKSettings;

        public SetupService(
            ISitecoreDataWrapper dataWrapper,
            IIBMWatsonApiKeys ibmApiKeys,
            IIntelligentTaggingSettings settings,
            IPublishWrapper publishWrapper,
            ISCSDKSettings scsdkSettings)
        {
            DataWrapper = dataWrapper;
            IBMApiKeys = ibmApiKeys;
            Settings = settings;
            PublishWrapper = publishWrapper;
            SCSDKSettings = scsdkSettings;
        }

        #endregion

        public void SaveKeysAndTest(string naturalLanguageClassifierEndpoint, string naturalLanguageClassifierUsername, string naturalLanguageClassifierPassword)
        {
            //save items to fields
            if (IBMApiKeys.NaturalLanguageClassifierEndpoint != naturalLanguageClassifierEndpoint)
                UpdateKey(SCSDKSettings.IBMSDK_NaturalLanguageClassifierEndpointFieldId, naturalLanguageClassifierEndpoint);
            if (IBMApiKeys.NaturalLanguageClassifierUsername != naturalLanguageClassifierUsername)
                UpdateKey(SCSDKSettings.IBMSDK_NaturalLanguageClassifierUsernameFieldId, naturalLanguageClassifierUsername);
            if (IBMApiKeys.NaturalLanguageClassifierPassword != naturalLanguageClassifierPassword)
                UpdateKey(SCSDKSettings.IBMSDK_NaturalLanguageClassifierPasswordFieldId, naturalLanguageClassifierPassword);
        }

        public void UpdateKey(ID fieldId, string value)
        {
            var keyItem = DataWrapper?
                .GetDatabase(SCSDKSettings.MasterDatabase)
                .GetItem(SCSDKSettings.IBMSDKId);
            DataWrapper.UpdateFields(keyItem, new Dictionary<ID, string>
            {
                { fieldId, value }
            });
        }

        public void PublishContent()
        {
            //publish templates folder for yourself and core, and publish scs root in modules
            List<ID> itemGuids = new List<ID>() {
                Settings.SCSDKTemplatesFolderId,
                Settings.TemplatesFolderId,
                Settings.SCSModulesFolderId
            };

            Database fromDb = DataWrapper.GetDatabase(Settings.MasterDatabase);
            Database toDb = DataWrapper.GetDatabase(Settings.WebDatabase);
            foreach (var g in itemGuids)
            {
                var folder = fromDb.GetItem(g);
                
                PublishWrapper.PublishItem(folder, new[] { toDb }, new[] { folder.Language }, true, false, false);
            }
        }
        
        public void ResetDictionary()
        { 
            Sitecore.Globalization.Translate.ResetCache(true);
        }
    }
}