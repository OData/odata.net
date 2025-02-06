namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _preferParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._prefer> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._prefer>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._prefer> Parse(IInput<char>? input)
            {
                var _ʺx50x72x65x66x65x72ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx50x72x65x66x65x72ʺParser.Instance.Parse(input);
if (!_ʺx50x72x65x66x65x72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._prefer)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_ʺx50x72x65x66x65x72ʺ_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._prefer)!, input);
}

var _OWS_1 = __GeneratedOdataV4.Parsers.Rules._OWSParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_OWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._prefer)!, input);
}

var _preference_1 = __GeneratedOdataV4.Parsers.Rules._preferenceParser.Instance.Parse(_OWS_1.Remainder);
if (!_preference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._prefer)!, input);
}

var _ⲤCOMMA_preferenceↃ_1 = Inners._ⲤCOMMA_preferenceↃParser.Instance.Many().Parse(_preference_1.Remainder);
if (!_ⲤCOMMA_preferenceↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._prefer)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._prefer(_ʺx50x72x65x66x65x72ʺ_1.Parsed, _ʺx3Aʺ_1.Parsed, _OWS_1.Parsed, _preference_1.Parsed, _ⲤCOMMA_preferenceↃ_1.Parsed), _ⲤCOMMA_preferenceↃ_1.Remainder);
            }
        }
    }
    
}
