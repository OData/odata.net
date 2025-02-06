namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectOptionPCParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC> Instance { get; } = (_filterParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC>(_searchParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC>(_inlinecountParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC>(_orderbyParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC>(_skipParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC>(_topParser.Instance);
        
        public static class _filterParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._filter> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._filter>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._filter> Parse(IInput<char>? input)
                {
                    var _filter_1 = __GeneratedOdataV4.Parsers.Rules._filterParser.Instance.Parse(input);
if (!_filter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectOptionPC._filter)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._filter(_filter_1.Parsed), _filter_1.Remainder);
                }
            }
        }
        
        public static class _searchParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._search> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._search>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._search> Parse(IInput<char>? input)
                {
                    var _search_1 = __GeneratedOdataV4.Parsers.Rules._searchParser.Instance.Parse(input);
if (!_search_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectOptionPC._search)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._search(_search_1.Parsed), _search_1.Remainder);
                }
            }
        }
        
        public static class _inlinecountParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._inlinecount> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._inlinecount>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._inlinecount> Parse(IInput<char>? input)
                {
                    var _inlinecount_1 = __GeneratedOdataV4.Parsers.Rules._inlinecountParser.Instance.Parse(input);
if (!_inlinecount_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectOptionPC._inlinecount)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._inlinecount(_inlinecount_1.Parsed), _inlinecount_1.Remainder);
                }
            }
        }
        
        public static class _orderbyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._orderby> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._orderby>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._orderby> Parse(IInput<char>? input)
                {
                    var _orderby_1 = __GeneratedOdataV4.Parsers.Rules._orderbyParser.Instance.Parse(input);
if (!_orderby_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectOptionPC._orderby)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._orderby(_orderby_1.Parsed), _orderby_1.Remainder);
                }
            }
        }
        
        public static class _skipParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._skip> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._skip>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._skip> Parse(IInput<char>? input)
                {
                    var _skip_1 = __GeneratedOdataV4.Parsers.Rules._skipParser.Instance.Parse(input);
if (!_skip_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectOptionPC._skip)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._skip(_skip_1.Parsed), _skip_1.Remainder);
                }
            }
        }
        
        public static class _topParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._top> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._top>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._top> Parse(IInput<char>? input)
                {
                    var _top_1 = __GeneratedOdataV4.Parsers.Rules._topParser.Instance.Parse(input);
if (!_top_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectOptionPC._top)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectOptionPC._top(_top_1.Parsed), _top_1.Remainder);
                }
            }
        }
    }
    
}
