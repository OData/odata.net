namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _simpleKeyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._simpleKey> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _ⲤparameterAliasⳆkeyPropertyValueↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤparameterAliasⳆkeyPropertyValueↃParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._simpleKey(_OPEN_1, _ⲤparameterAliasⳆkeyPropertyValueↃ_1, _CLOSE_1);
    }
    
}
