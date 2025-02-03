namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_qualifiedComplexTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedComplexTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedComplexTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName(_ʺx2Fʺ_1, _qualifiedComplexTypeName_1);
    }
    
}
