namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _entitySetNameParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._entitySetName> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entitySetName(_odataIdentifier_1);

        //// PERF
        //// public static IParser<char, __GeneratedOdata.CstNodes.Rules._entitySetName> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._entitySetName>
        {
            public IOutput<char, _entitySetName> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._entitySetName(_odataIdentifier_1.Parsed),
                    _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
