namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandCountOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandCountOption> Instance { get; } = (_filterParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._expandCountOption>(_searchParser.Instance);
        
        public static class _filterParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandCountOption._filter> Instance { get; } = from _filter_1 in __GeneratedOdataV3.Parsers.Rules._filterParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._expandCountOption._filter(_filter_1);
        }
        
        public static class _searchParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandCountOption._search> Instance { get; } = from _search_1 in __GeneratedOdataV3.Parsers.Rules._searchParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._expandCountOption._search(_search_1);
        }
    }
    
}
