namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityNavigationPropertyParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityNavigationProperty> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdataV2.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityNavigationProperty(_odataIdentifier_1);
    }
    
}
