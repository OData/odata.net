namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandCountOptionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption> Instance { get; } = (_filterParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption>(_searchParser.Instance);
        
        public static class _filterParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption._filter> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption._filter>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption._filter> Parse(IInput<char>? input)
                {
                    var _filter_1 = __GeneratedOdataV4.Parsers.Rules._filterParser.Instance.Parse(input);
if (!_filter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandCountOption._filter)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandCountOption._filter(_filter_1.Parsed), _filter_1.Remainder);
                }
            }
        }
        
        public static class _searchParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption._search> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption._search>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandCountOption._search> Parse(IInput<char>? input)
                {
                    var _search_1 = __GeneratedOdataV4.Parsers.Rules._searchParser.Instance.Parse(input);
if (!_search_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandCountOption._search)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandCountOption._search(_search_1.Parsed), _search_1.Remainder);
                }
            }
        }
    }
    
}
