namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤsearchPhraseⳆsearchWordↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤsearchPhraseⳆsearchWordↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤsearchPhraseⳆsearchWordↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤsearchPhraseⳆsearchWordↃ> Parse(IInput<char>? input)
            {
                var _searchPhraseⳆsearchWord_1 = __GeneratedOdataV3.Parsers.Inners._searchPhraseⳆsearchWordParser.Instance.Parse(input);
if (!_searchPhraseⳆsearchWord_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤsearchPhraseⳆsearchWordↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤsearchPhraseⳆsearchWordↃ(_searchPhraseⳆsearchWord_1.Parsed), _searchPhraseⳆsearchWord_1.Remainder);
            }
        }
    }
    
}
