namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _compoundKeyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._compoundKey> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._compoundKey>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._compoundKey> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compoundKey)!, input);
}

var _keyValuePair_1 = __GeneratedOdataV3.Parsers.Rules._keyValuePairParser.Instance.Parse(_OPEN_1.Remainder);
if (!_keyValuePair_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compoundKey)!, input);
}

var _ⲤCOMMA_keyValuePairↃ_1 = Inners._ⲤCOMMA_keyValuePairↃParser.Instance.Many().Parse(_keyValuePair_1.Remainder);
if (!_ⲤCOMMA_keyValuePairↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compoundKey)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤCOMMA_keyValuePairↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compoundKey)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._compoundKey(_OPEN_1.Parsed, _keyValuePair_1.Parsed, _ⲤCOMMA_keyValuePairↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
