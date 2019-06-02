using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Services
{
    public class ContentSearchService : IContentSearchService
    {
        #region Constructor
        
        protected readonly IIntelligentTaggingSettings Settings;
        protected readonly IContentSearchWrapper ContentSearch;

        public ContentSearchService(
            IIntelligentTaggingSettings settings,
            IContentSearchWrapper contentSearch)
        {
            Settings = settings;
            ContentSearch = contentSearch;
        }

        #endregion
        
        public List<SearchResultItem> GetContent(string database, string language, List<ID> templateIds, int take, int skip = 0)
        {
            var templatePredicate = PredicateBuilder.False<SearchResultItem>();
            foreach (var t in templateIds)
            {
                templatePredicate = templatePredicate.Or(x => x.TemplateId == t);
            }

            var index = ContentSearch.GetIndex(ContentSearch.GetSitecoreIndexName(database));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                var results = context.GetQueryable<SearchResultItem>()
                    .Where(templatePredicate)
                    .Where(a =>
                        a.Paths.Contains(Settings.ContentNodeId)
                        && a.Language == language)
                    .OrderBy(a => a.ItemId)
                    .Skip(skip)
                    .Take(take);

                return results.ToList();
            }
        }
        
        public List<SearchResultItem> GetTags(ID tagSourceId, string language, string database, List<string> tags)
        {
            var tagPredicate = PredicateBuilder.False<SearchResultItem>();
            foreach (var t in tags)
            {
                tagPredicate = tagPredicate.Or(x => x.Name == t);
            }

            var index = ContentSearch.GetIndex(ContentSearch.GetSitecoreIndexName(database));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                var results = context.GetQueryable<SearchResultItem>()
                    .Where(tagPredicate)
                    .Where(a =>
                        a.Paths.Contains(tagSourceId)
                        && a.Language == language);

                return results.ToList();
            }
        }

        public List<SearchResultItem> GetTagsByTemplate(ID tagSourceId, string language, string database, List<ID> templateIds)
        {
            var templatePredicate = PredicateBuilder.False<SearchResultItem>();
            foreach (var t in templateIds)
            {
                templatePredicate = templatePredicate.Or(x => x.TemplateId == t);
            }

            var index = ContentSearch.GetIndex(ContentSearch.GetSitecoreIndexName(database));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                var results = context.GetQueryable<SearchResultItem>()
                    .Where(templatePredicate)
                    .Where(a =>
                        a.Paths.Contains(tagSourceId)
                        && a.Language == language);

                return results.ToList();
            }
        }
    }
}