namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _expandCountOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._expandCountOption> Instance { get; } = (_filterParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandCountOption>(_searchParser.Instance);
        
        public static class _filterParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandCountOption._filter> Instance { get; } = from _filter_1 in __GeneratedOdata.Parsers.Rules._filterParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandCountOption._filter(_filter_1);
        }
        
        public static class _searchParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandCountOption._search> Instance { get; } = from _search_1 in __GeneratedOdata.Parsers.Rules._searchParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandCountOption._search(_search_1);
        }
    }
    
}