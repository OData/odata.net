namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyValuePairParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyValuePair> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyValuePair>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._keyValuePair> Parse(IInput<char>? input)
            {
                var _ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃParser.Instance.Parse(input);
if (!_ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._keyValuePair)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._keyValuePair)!, input);
}

var _ⲤparameterAliasⳆkeyPropertyValueↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤparameterAliasⳆkeyPropertyValueↃParser.Instance.Parse(_EQ_1.Remainder);
if (!_ⲤparameterAliasⳆkeyPropertyValueↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._keyValuePair)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._keyValuePair(_ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ_1.Parsed, _EQ_1.Parsed,  _ⲤparameterAliasⳆkeyPropertyValueↃ_1.Parsed), _ⲤparameterAliasⳆkeyPropertyValueↃ_1.Remainder);
            }
        }
    }
    
}
