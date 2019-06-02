using System;
using System.Linq;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Commands
{
    [Serializable]
    public class LearnType : BaseIntelligentTaggingCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
            
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentTagging/LearnType?id={Id}&language={Language}&db={Db}")
            {
                Header = "Learn Type",
                Height = "500",
                Width = "810",
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
            if (ctxItem == null || !ctxItem.Paths.FullPath.StartsWith("/sitecore/content"))
                return CommandState.Hidden;

            if(ctxItem.Database.Name == "core")
                return CommandState.Hidden;

            var mappings = ContentService.GetContentMappings(ctxItem.Database.Name);
            if (mappings.Values.Any(b => b.TemplateField.Guid == ctxItem.TemplateID.Guid))
                return CommandState.Hidden;

            return CommandState.Enabled;
        }
    }
}