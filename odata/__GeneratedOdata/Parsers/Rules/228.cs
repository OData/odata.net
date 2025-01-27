namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEnumTypeNameParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._qualifiedEnumTypeName> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _enumerationTypeName_1 in __GeneratedOdata.Parsers.Rules._enumerationTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qualifiedEnumTypeName(_namespace_1, _ʺx2Eʺ_1, _enumerationTypeName_1);
    }
    
}
