namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _parameterAliasParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._parameterAlias> Instance { get; } = from _AT_1 in __GeneratedOdata.Parsers.Rules._ATParser.Instance
from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._parameterAlias(_AT_1, _odataIdentifier_1);
    }
    
}
