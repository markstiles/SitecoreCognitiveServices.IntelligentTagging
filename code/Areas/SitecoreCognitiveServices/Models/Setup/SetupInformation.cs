namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Setup
{
    public class SetupInformation : ISetupInformation
    {
        public string NaturalLanguageClassifier { get; set; }
        public string NaturalLanguageClassifierEndpoint { get; set; }
        public string NaturalLanguageClassifierUsername { get; set; }
        public string NaturalLanguageClassifierPassword { get; set; }
    }
}