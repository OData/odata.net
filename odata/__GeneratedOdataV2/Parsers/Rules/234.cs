namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._complexTypeName> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdataV2.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._complexTypeName(_odataIdentifier_1);
    }
    
}
