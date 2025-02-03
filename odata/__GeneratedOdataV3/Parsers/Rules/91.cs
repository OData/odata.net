namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectOptionPCParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC> Instance { get; } = (_filterParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC>(_searchParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC>(_inlinecountParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC>(_orderbyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC>(_skipParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC>(_topParser.Instance);
        
        public static class _filterParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._filter> Instance { get; } = from _filter_1 in __GeneratedOdataV3.Parsers.Rules._filterParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._filter(_filter_1);
        }
        
        public static class _searchParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._search> Instance { get; } = from _search_1 in __GeneratedOdataV3.Parsers.Rules._searchParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._search(_search_1);
        }
        
        public static class _inlinecountParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._inlinecount> Instance { get; } = from _inlinecount_1 in __GeneratedOdataV3.Parsers.Rules._inlinecountParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._inlinecount(_inlinecount_1);
        }
        
        public static class _orderbyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._orderby> Instance { get; } = from _orderby_1 in __GeneratedOdataV3.Parsers.Rules._orderbyParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._orderby(_orderby_1);
        }
        
        public static class _skipParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._skip> Instance { get; } = from _skip_1 in __GeneratedOdataV3.Parsers.Rules._skipParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._skip(_skip_1);
        }
        
        public static class _topParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._top> Instance { get; } = from _top_1 in __GeneratedOdataV3.Parsers.Rules._topParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOptionPC._top(_top_1);
        }
    }
    
}
