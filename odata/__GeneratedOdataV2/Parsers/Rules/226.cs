namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedComplexTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedComplexTypeName> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _complexTypeName_1 in __GeneratedOdataV2.Parsers.Rules._complexTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qualifiedComplexTypeName(_namespace_1, _ʺx2Eʺ_1, _complexTypeName_1);
    }
    
}
