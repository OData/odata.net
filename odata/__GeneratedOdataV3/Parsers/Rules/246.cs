namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveKeyPropertyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveKeyProperty> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdataV3.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveKeyProperty(_odataIdentifier_1);
    }
    
}
