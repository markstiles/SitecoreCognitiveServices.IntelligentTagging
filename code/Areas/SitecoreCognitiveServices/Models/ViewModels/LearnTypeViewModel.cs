using System.Collections.Generic;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.ViewModels
{
    public class LearnTypeViewModel
    {
        public List<string> Fields { get; set; }

        public LearnTypeViewModel()
        {
            Fields = new List<string>();
        }
    }
}