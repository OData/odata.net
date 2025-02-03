namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _simpleKeyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._simpleKey> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._simpleKey>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._simpleKey> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._simpleKey)!, input);
}

var _ⲤparameterAliasⳆkeyPropertyValueↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤparameterAliasⳆkeyPropertyValueↃParser.Instance.Parse(_OPEN_1.Remainder);
if (!_ⲤparameterAliasⳆkeyPropertyValueↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._simpleKey)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤparameterAliasⳆkeyPropertyValueↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._simpleKey)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._simpleKey(_OPEN_1.Parsed, _ⲤparameterAliasⳆkeyPropertyValueↃ_1.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
