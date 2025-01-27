namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityColFunctionParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._entityColFunction> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityColFunction(_odataIdentifier_1);
    }
    
}
