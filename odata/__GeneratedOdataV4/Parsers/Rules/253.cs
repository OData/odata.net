namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityNavigationPropertyParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdataV4.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);
if (!_odataIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty(_odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
