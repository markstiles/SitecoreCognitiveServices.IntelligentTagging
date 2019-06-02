using System;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Commands
{
    [Serializable]
    public class TrainClassifier : BaseIntelligentTaggingCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
      
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentTagging/TrainClassifier?id={Id}&language={Language}&db={Db}")
            {
                Header = "Train Classifier",
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