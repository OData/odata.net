namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _searchPhraseⳆsearchWordParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord> Instance { get; } = (_searchPhraseParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord>(_searchWordParser.Instance);
        
        public static class _searchPhraseParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase> Parse(IInput<char>? input)
                {
                    var _searchPhrase_1 = __GeneratedOdataV4.Parsers.Rules._searchPhraseParser.Instance.Parse(input);
if (!_searchPhrase_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase(_searchPhrase_1.Parsed), _searchPhrase_1.Remainder);
                }
            }
        }
        
        public static class _searchWordParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord> Parse(IInput<char>? input)
                {
                    var _searchWord_1 = __GeneratedOdataV4.Parsers.Rules._searchWordParser.Instance.Parse(input);
if (!_searchWord_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord(_searchWord_1.Parsed), _searchWord_1.Remainder);
                }
            }
        }
    }
    
}
