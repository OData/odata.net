namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEnumTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedEnumTypeName> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _enumerationTypeName_1 in __GeneratedOdataV2.Parsers.Rules._enumerationTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qualifiedEnumTypeName(_namespace_1, _ʺx2Eʺ_1, _enumerationTypeName_1);
    }
    
}
