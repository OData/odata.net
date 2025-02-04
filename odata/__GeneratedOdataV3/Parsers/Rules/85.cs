namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchPhraseParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._searchPhrase> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._searchPhrase>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._searchPhrase> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchPhrase)!, input);
}

var _qcharⲻnoⲻAMPⲻDQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._qcharⲻnoⲻAMPⲻDQUOTEParser.Instance.Repeat(1, null).Parse(_quotationⲻmark_1.Remainder);
if (!_qcharⲻnoⲻAMPⲻDQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchPhrase)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_qcharⲻnoⲻAMPⲻDQUOTE_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchPhrase)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._searchPhrase(_quotationⲻmark_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE>(_qcharⲻnoⲻAMPⲻDQUOTE_1.Parsed), _quotationⲻmark_2.Parsed), _quotationⲻmark_2.Remainder);
            }
        }
    }
    
}
