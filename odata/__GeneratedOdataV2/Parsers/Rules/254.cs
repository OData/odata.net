namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityColNavigationPropertyParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityColNavigationProperty> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdataV2.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityColNavigationProperty(_odataIdentifier_1);
    }
    
}
