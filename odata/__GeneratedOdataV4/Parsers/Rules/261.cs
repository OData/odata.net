namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColFunctionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexColFunction> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexColFunction>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._complexColFunction> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdataV4.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);
if (!_odataIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexColFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._complexColFunction(_odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
