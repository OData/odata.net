namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_qualifiedComplexTypeNameParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedComplexTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName(_ʺx2Fʺ_1, _qualifiedComplexTypeName_1);
    }
    
}
