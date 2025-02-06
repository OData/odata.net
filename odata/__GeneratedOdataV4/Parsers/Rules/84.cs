namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchTermParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._searchTerm> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._searchTerm>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._searchTerm> Parse(IInput<char>? input)
            {
                var _ʺx4Ex4Fx54ʺ_RWS_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Ex4Fx54ʺ_RWSParser.Instance.Optional().Parse(input);
if (!_ʺx4Ex4Fx54ʺ_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._searchTerm)!, input);
}

var _ⲤsearchPhraseⳆsearchWordↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤsearchPhraseⳆsearchWordↃParser.Instance.Parse(_ʺx4Ex4Fx54ʺ_RWS_1.Remainder);
if (!_ⲤsearchPhraseⳆsearchWordↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._searchTerm)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._searchTerm(_ʺx4Ex4Fx54ʺ_RWS_1.Parsed.GetOrElse(null), _ⲤsearchPhraseⳆsearchWordↃ_1.Parsed), _ⲤsearchPhraseⳆsearchWordↃ_1.Remainder);
            }
        }
    }
    
}
