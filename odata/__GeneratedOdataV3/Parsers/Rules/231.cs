namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entitySetNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._entitySetName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._entitySetName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._entitySetName> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdataV3.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._entitySetName(_odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
