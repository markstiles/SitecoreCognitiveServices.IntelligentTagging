using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Rules;
using SitecoreCognitiveServices.Feature.IntelligentTagging.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers
{
    public class RulesClassifier : BaseClassifier, IClassProvider
    {
        #region Constructor

        protected readonly IContentSearchService ContentSearchService;

        public RulesClassifier(IServiceProvider provider, Item classifierItem) : base(provider, classifierItem)
        {
            ContentSearchService = provider.GetService<IContentSearchService>();
        }

        #endregion

        #region IClassProvider

        public List<string> Classify(Item itemToTag, string text)
        {
            var tags = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
                return tags;

            //get all the tags
            var tagItems = ContentSearchService.GetTagsByTemplate(TaxonomyFolderId, InnerItem.Language.Name, InnerItem.Database.Name, TaxonomyItemTemplateIds);

            //loop through and pull out the rules
            foreach (var t in tagItems)
            {
                var rulesField = t.GetItem().Fields[Settings.RulesFieldId];
                if (rulesField == null)
                    continue;
                
                var rules = RuleFactory.GetRules<RuleContext>(rulesField);

                //run rule against the content
                var ruleContext = new RuleContext { Item = itemToTag };
                if(EvaluateRules(rules, ruleContext))
                    tags.Add(t.Name);
            }

            return tags;
        }

        #endregion

        public bool EvaluateRules(RuleList<RuleContext> ruleList, RuleContext ruleContext)
        {
            if (ruleList == null || ruleList.Count == 0 || ruleContext == null)
                return false;

            foreach (var rule in ruleList.Rules)
            {
                if (rule.Condition == null)
                    continue;

                var isValid = rule.Evaluate(ruleContext);
                if (!isValid)
                    return false;
            }

            return true;
        }
    }
}