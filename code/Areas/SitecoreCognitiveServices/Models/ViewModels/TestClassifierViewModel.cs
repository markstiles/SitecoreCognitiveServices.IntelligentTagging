using System.Collections.Generic;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.ViewModels
{
    public class TestClassifierViewModel
    {
        public bool Failed { get; set; }
        public string Message { get; set; }

        public TestClassifierViewModel(bool failed, string message = "")
        {
            Failed = failed;
            Message = message;
        }
    }
}