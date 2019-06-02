using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Services
{
    public interface IContentSearchService
    {
        List<SearchResultItem> GetContent(string database, string language, List<ID> templateIds, int take, int skip = 0);
        List<SearchResultItem> GetTags(ID tagSourceId, string language, string database, List<string> tags);
        List<SearchResultItem> GetTagsByTemplate(ID tagSourceId, string language, string database, List<ID> templateIds);
    }
}