using System;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Commands
{
    [Serializable]
    public class TestClassifier : BaseIntelligentTaggingCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
      
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentTagging/TestClassifier?id={Id}&language={Language}&db={Db}")
            {
                Header = "Test Classifier",
                Height = "250",
                Width = "410",
                Message = "",
                Response = true
            };
            SheerResponse.ShowModalDialog(mdo);
            args.WaitForPostBack();
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (Settings.MissingKeys())
                return CommandState.Disabled;

            Item ctxItem = DataWrapper?.ExtractItem(context);
            if (ctxItem == null || ctxItem.TemplateID.Guid != Settings.ClassifierTemplateId.Guid)
                return CommandState.Hidden;

            var classifier = ClassifierFactory.Create(ctxItem);
            var isTrainable = classifier is IClassTrainer;
            return (isTrainable)
                ? CommandState.Enabled
                : CommandState.Hidden;
        }
    }
}