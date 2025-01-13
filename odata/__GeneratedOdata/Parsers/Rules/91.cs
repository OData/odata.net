namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _selectOptionPCParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._selectOptionPC> Instance { get; } = (_filterParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectOptionPC>(_searchParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectOptionPC>(_inlinecountParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectOptionPC>(_orderbyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectOptionPC>(_skipParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectOptionPC>(_topParser.Instance);
        
        public static class _filterParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectOptionPC._filter> Instance { get; } = from _filter_1 in __GeneratedOdata.Parsers.Rules._filterParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectOptionPC._filter(_filter_1);
        }
        
        public static class _searchParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectOptionPC._search> Instance { get; } = from _search_1 in __GeneratedOdata.Parsers.Rules._searchParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectOptionPC._search(_search_1);
        }
        
        public static class _inlinecountParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectOptionPC._inlinecount> Instance { get; } = from _inlinecount_1 in __GeneratedOdata.Parsers.Rules._inlinecountParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectOptionPC._inlinecount(_inlinecount_1);
        }
        
        public static class _orderbyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectOptionPC._orderby> Instance { get; } = from _orderby_1 in __GeneratedOdata.Parsers.Rules._orderbyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectOptionPC._orderby(_orderby_1);
        }
        
        public static class _skipParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectOptionPC._skip> Instance { get; } = from _skip_1 in __GeneratedOdata.Parsers.Rules._skipParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectOptionPC._skip(_skip_1);
        }
        
        public static class _topParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectOptionPC._top> Instance { get; } = from _top_1 in __GeneratedOdata.Parsers.Rules._topParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectOptionPC._top(_top_1);
        }
    }
    
}
