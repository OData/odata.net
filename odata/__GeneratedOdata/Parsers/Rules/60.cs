namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _systemQueryOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption> Instance { get; } = (_computeParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_deltatokenParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_expandParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_filterParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_formatParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_idParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_inlinecountParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_orderbyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_schemaversionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_searchParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_selectParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_skipParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_skiptokenParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_topParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._systemQueryOption>(_indexParser.Instance);
        
        public static class _computeParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._compute> Instance { get; } = from _compute_1 in __GeneratedOdata.Parsers.Rules._computeParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._compute(_compute_1);
        }
        
        public static class _deltatokenParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._deltatoken> Instance { get; } = from _deltatoken_1 in __GeneratedOdata.Parsers.Rules._deltatokenParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._deltatoken(_deltatoken_1);
        }
        
        public static class _expandParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._expand> Instance { get; } = from _expand_1 in __GeneratedOdata.Parsers.Rules._expandParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._expand(_expand_1);
        }
        
        public static class _filterParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._filter> Instance { get; } = from _filter_1 in __GeneratedOdata.Parsers.Rules._filterParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._filter(_filter_1);
        }
        
        public static class _formatParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._format> Instance { get; } = from _format_1 in __GeneratedOdata.Parsers.Rules._formatParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._format(_format_1);
        }
        
        public static class _idParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._id> Instance { get; } = from _id_1 in __GeneratedOdata.Parsers.Rules._idParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._id(_id_1);
        }
        
        public static class _inlinecountParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._inlinecount> Instance { get; } = from _inlinecount_1 in __GeneratedOdata.Parsers.Rules._inlinecountParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._inlinecount(_inlinecount_1);
        }
        
        public static class _orderbyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._orderby> Instance { get; } = from _orderby_1 in __GeneratedOdata.Parsers.Rules._orderbyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._orderby(_orderby_1);
        }
        
        public static class _schemaversionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._schemaversion> Instance { get; } = from _schemaversion_1 in __GeneratedOdata.Parsers.Rules._schemaversionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._schemaversion(_schemaversion_1);
        }
        
        public static class _searchParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._search> Instance { get; } = from _search_1 in __GeneratedOdata.Parsers.Rules._searchParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._search(_search_1);
        }
        
        public static class _selectParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._select> Instance { get; } = from _select_1 in __GeneratedOdata.Parsers.Rules._selectParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._select(_select_1);
        }
        
        public static class _skipParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._skip> Instance { get; } = from _skip_1 in __GeneratedOdata.Parsers.Rules._skipParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._skip(_skip_1);
        }
        
        public static class _skiptokenParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._skiptoken> Instance { get; } = from _skiptoken_1 in __GeneratedOdata.Parsers.Rules._skiptokenParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._skiptoken(_skiptoken_1);
        }
        
        public static class _topParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._top> Instance { get; } = from _top_1 in __GeneratedOdata.Parsers.Rules._topParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._top(_top_1);
        }
        
        public static class _indexParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._systemQueryOption._index> Instance { get; } = from _index_1 in __GeneratedOdata.Parsers.Rules._indexParser.Instance
select new __GeneratedOdata.CstNodes.Rules._systemQueryOption._index(_index_1);
        }
    }
    
}