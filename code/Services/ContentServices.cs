using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Areas.SitecoreCognitiveServices.Models.Classification;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Services
{
    public class ContentService : IContentService
    {
        #region Constructor

        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentTaggingSettings Settings;
        protected readonly IContentSearchWrapper ContentSearch;
        protected readonly IContentMappingFactory ContentMappingFactory;
        protected readonly IClassifierFactory ClassifierFactory;

        public ContentService(
            ISitecoreDataWrapper dataWrapper,
            IIntelligentTaggingSettings settings,
            IContentSearchWrapper contentSearch,
            IContentMappingFactory contentMappingFactory,
            IClassifierFactory classifierFactory)
        {
            DataWrapper = dataWrapper;
            Settings = settings;
            ContentSearch = contentSearch;
            ContentMappingFactory = contentMappingFactory;
            ClassifierFactory = classifierFactory;
        }

        #endregion
        
        public List<IBaseClassifier> GetClassifiers(string database)
        {
            var classifiers = DataWrapper
                .GetItemById(Settings.ClassifiersFolderId, database)
                .GetChildren()
                .Select(a => ClassifierFactory.Create(a))
                .Where(b => b != null)
                .ToList();

            return classifiers;
        }
        
        public Dictionary<Guid, IContentMapping> GetContentMappings(string database)
        {
            var mappings = DataWrapper
                .GetItemById(Settings.ContentMappingsFolderId, database)
                .GetChildren()
                .Select(ContentMappingFactory.Create)
                .Where(a => a != null)
                .ToDictionary(a => a.TemplateField.Guid);

            return mappings;
        }

        public Item AddMapping(string database, string newItemTemplateName, List<string> contentFieldIds, string templateId)
        {
            Item mappingFolder = DataWrapper.GetItemById(Settings.ContentMappingsFolderId, database);
            if (mappingFolder == null)
                return null;

            Item possibleMatch = mappingFolder.Axes.GetChild(newItemTemplateName);
            if (possibleMatch != null)
                return null;

            Dictionary<ID, string> fields = new Dictionary<ID, string>
            {
                { Settings.ContentFieldsId, string.Join("|", contentFieldIds) },
                { Settings.TemplateFieldId, templateId }
            };

            return DataWrapper.CreateItem(Settings.ContentMappingsFolderId, Settings.ContentMappingTemplateId, database, newItemTemplateName, fields);
        }
        
        public string GetTrainingData(Item contentItem, ID sourceTagsField, List<ID> contentFieldIds)
        {
            var tags = GetTags(contentItem, sourceTagsField);
            var content = GetTrimmedContent(contentItem, contentFieldIds);
            if (!tags.Any() || string.IsNullOrWhiteSpace(content))
                return string.Empty;

            return $"\"{content}\",{string.Join(",", tags)}";
        }

        public List<string> GetTags(Item item, ID sourceTagsField)
        {
            item.Fields.ReadAll();
            DelimitedField df = item?.Fields[sourceTagsField];
            if (df == null)
                return new List<string>();

            var tagNames = df.Items
                .Select(t => item.Database.GetItem(t)?.Name)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToList();

            return tagNames;
        }

        public string GetContent(Item item, List<ID> contentFieldIds)
        {
            item.Fields.ReadAll();
            var contentValues = contentFieldIds
                .Select(c => StripInvalidChars(item.Fields[c]?.Value) ?? string.Empty)
                .Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

            var textValue = contentValues.Any()
                ? string.Join(" ", contentValues).Trim()
                : string.Empty;

            return textValue;
        }

        public string GetTrimmedContent(Item item, List<ID> contentFieldIds)
        {
            var textValue = GetContent(item, contentFieldIds);

            if (textValue.Length > 1022)
                textValue = textValue.Substring(0, 1022);

            return textValue;
        }

        public string StripInvalidChars(string htmlString)
        {
            htmlString = htmlString.ToLower();

            string htmlMarkupPattern = @"<(.|\n)*?>";
            string htmlEntitiesPattern = @"&.{2,5}[\;]";
            string punctuationPattern = @"[^\w\s']";
            string spacePattern = @"\s+";

            htmlString = Regex.Replace(htmlString, htmlMarkupPattern, " ");
            htmlString = Regex.Replace(htmlString, htmlEntitiesPattern, " ");
            htmlString = Regex.Replace(htmlString, punctuationPattern, " ");

            List<string> stopWords = new List<string>
            {
                "a", "able", "about", "above", "abst", "accordance", "according", "accordingly", "across", "act", "actually", "added", "adj", "affected", "affecting", "affects", "after", "afterwards", "again", "against", "ah", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "an", "and", "announce", "another", "any", "anybody", "anyhow", "anymore", "anyone", "anything", "anyway", "anyways", "anywhere", "apparently", "approximately", "are", "aren", "arent", "aren't", "arise", "around", "as", "aside", "ask", "asking", "at", "auth", "available", "away", "awfully",
                "b", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "begin", "beginning", "beginnings", "begins", "behind", "being", "believe", "below", "beside", "besides", "between", "beyond", "biol", "both", "brief", "briefly", "but", "by",
                "c", "ca", "came", "can", "cannot", "can't", "cause", "causes", "certain", "certainly", "co", "com", "come", "comes", "contain", "containing", "contains", "could", "couldnt",
                "d", "date", "did", "didn't", "different", "do", "does", "doesn't", "doing", "done", "don't", "down", "downwards", "due", "during",
                "e", "each", "ed", "edu", "effect", "eg", "eight", "eighty", "either", "else", "elsewhere", "end", "ending", "enough", "especially", "et", "et-al", "etc", "even", "ever", "every", "everybody", "everyone", "everything", "everywhere", "ex", "except",
                "f", "far", "few", "ff", "fifth", "first", "five", "fix", "followed", "following", "follows", "for", "former", "formerly", "forth", "found", "four", "from", "further", "furthermore",
                "g", "gave", "get", "gets", "getting", "give", "given", "gives", "giving", "go", "goes", "gone", "got", "gotten",
                "h", "had", "happens", "hardly", "has", "hasn't", "have", "haven't", "having", "he", "hed", "hence", "her", "here", "hereafter", "hereby", "herein", "heres", "hereupon", "hers", "herself", "he's", "hes", "hi", "hid", "him", "himself", "his", "hither", "home", "how", "howbeit", "however", "hundred",
                "i", "id", "ie", "if", "i'll", "im", "immediate", "immediately", "importance", "important", "in", "inc", "indeed", "index", "information", "instead", "into", "invention", "inward", "is", "isn't", "it", "itd", "it'd", "it'll", "its", "itself", "i've",
                "j", "just",
                "k", "keep", "keeps", "kept", "kg", "km", "know", "known", "knows",
                "l", "largely", "last", "lately", "later", "latter", "latterly", "least", "less", "lest", "let", "let's", "lets", "like", "liked", "likely", "line", "little", "'ll", "look", "looking", "looks", "ltd",
                "m", "made", "mainly", "make", "makes", "many", "may", "maybe", "me", "mean", "means", "meantime", "meanwhile", "merely", "mg", "might", "million", "miss", "ml", "more", "moreover", "most", "mostly", "mr", "mrs", "much", "mug", "must", "my", "myself",
                "n", "na", "name", "namely", "nay", "nd", "near", "nearly", "necessarily", "necessary", "need", "needs", "neither", "never", "nevertheless", "new", "next", "nine", "ninety", "no", "nobody", "non", "none", "nonetheless", "noone", "nor", "normally", "nos", "not", "noted", "nothing", "now", "nowhere",
                "o", "obtain", "obtained", "obviously", "of", "off", "often", "oh", "ok", "okay", "old", "omitted", "on", "once", "one", "ones", "only", "onto", "or", "ord", "other", "others", "otherwise", "ought", "our", "ours", "ourselves", "out", "outside", "over", "overall", "owing", "own",
                "p", "page", "pages", "part", "particular", "particularly", "past", "per", "perhaps", "placed", "please", "plus", "poorly", "possible", "possibly", "potentially", "pp", "predominantly", "present", "previously", "primarily", "probably", "promptly", "proud", "provides", "put",
                "q", "que", "quickly", "quite", "qv",
                "r", "ran", "rather", "rd", "re", "readily", "really", "recent", "recently", "ref", "refs", "regarding", "regardless", "regards", "related", "relatively", "research", "respectively", "resulted", "resulting", "results", "right", "run",
                "s", "said", "same", "saw", "say", "saying", "says", "sec", "section", "see", "seeing", "seem", "seemed", "seeming", "seems", "seen", "self", "selves", "sent", "seven", "several", "shall", "she", "shed", "she'll", "shes", "should", "shouldn't", "show", "showed", "shown", "showns", "shows", "significant", "significantly", "similar", "similarly", "since", "six", "slightly", "so", "some", "somebody", "somehow", "someone", "somethan", "something", "sometime", "sometimes", "somewhat", "somewhere", "soon", "sorry", "specifically", "specified", "specify", "specifying", "still", "stop", "strongly", "sub", "substantially", "successfully", "such", "sufficiently", "suggest", "sup", "sure",
                "t", "take", "taken", "taking", "tell", "tends", "th", "than", "thank", "thanks", "thanx", "that", "that'll", "thats", "that've", "the", "their", "theirs", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "there'd", "thered", "therefore", "therein", "there'll", "thereof", "therere", "theres", "thereto", "thereupon", "there've", "these", "they", "they'd", "theyd", "they'll", "they're", "theyre", "they've", "think", "this", "those", "thou", "though", "thoughh", "thousand", "throug", "through", "throughout", "thru", "thus", "til", "tip", "to", "together", "too", "took", "toward", "towards", "tried", "tries", "truly", "try", "trying", "ts", "twice", "two",
                "u", "un", "under", "unfortunately", "unless", "unlike", "unlikely", "until", "unto", "up", "upon", "ups", "us", "use", "used", "useful", "usefully", "usefulness", "uses", "using", "usually",
                "v", "value", "various", "'ve", "very", "via", "viz", "vol", "vols", "vs",
                "w", "want", "wants", "was", "wasnt", "way", "we", "wed", "welcome", "we'll", "went", "were", "weren't", "werent", "we've", "what", "whatever", "what'll", "whats", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "wheres", "whereupon", "wherever", "whether", "which", "while", "whim", "whither", "who", "whod", "whoever", "whole", "who'll", "whom", "whomever", "whos", "whose", "why", "widely", "willing", "wish", "with", "within", "without", "wont", "words", "world", "would", "wouldnt", "www",
                "x",
                "y", "yes", "yet", "you", "you'd", "youd", "you'll", "your", "youre", "yours", "yourself", "yourselves", "you've",
                "z", "zero"
            };
            foreach (var w in stopWords)
            {
                htmlString = htmlString.Replace($" {w} ", " ");
            }

            htmlString = Regex.Replace(htmlString, spacePattern, " ");

            return htmlString.Trim();
        }
    }
}