namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._parameterAlias> Instance { get; } = from _AT_1 in __GeneratedOdataV2.Parsers.Rules._ATParser.Instance
from _odataIdentifier_1 in __GeneratedOdataV2.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._parameterAlias(_AT_1, _odataIdentifier_1);
    }
    
}
