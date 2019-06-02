namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Setup
{
    public interface ISetupInformation
    {
        string NaturalLanguageClassifierEndpoint { get; set; }
        string NaturalLanguageClassifierUsername { get; set; }
        string NaturalLanguageClassifierPassword { get; set; }
    }
}