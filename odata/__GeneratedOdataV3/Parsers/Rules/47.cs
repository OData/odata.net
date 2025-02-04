namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._parameterAlias> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._parameterAlias>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._parameterAlias> Parse(IInput<char>? input)
            {
                var _AT_1 = __GeneratedOdataV3.Parsers.Rules._ATParser.Instance.Parse(input);
if (!_AT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._parameterAlias)!, input);
}

var _odataIdentifier_1 = __GeneratedOdataV3.Parsers.Rules._odataIdentifierParser.Instance.Parse(_AT_1.Remainder);
if (!_odataIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._parameterAlias)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._parameterAlias(_AT_1.Parsed, _odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
