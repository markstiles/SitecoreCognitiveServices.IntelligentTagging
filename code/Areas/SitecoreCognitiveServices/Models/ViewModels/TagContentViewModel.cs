using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.ViewModels
{
    public class TagContentViewModel
    {
        public string Content { get; set; }
        public List<ListItem> Classifiers { get; set; }

        public TagContentViewModel(string content, List<ListItem> classifiers)
        {
            Content = content;
            Classifiers = classifiers;
        }
    }
}