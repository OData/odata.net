namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _entityColNavigationPropertyParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._entityColNavigationProperty> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityColNavigationProperty(_odataIdentifier_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._entityColNavigationProperty> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._entityColNavigationProperty>
        {
            public IOutput<char, _entityColNavigationProperty> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);
                if (!_odataIdentifier_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_entityColNavigationProperty)!,
                        input);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._entityColNavigationProperty(_odataIdentifier_1.Parsed),
                    _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
