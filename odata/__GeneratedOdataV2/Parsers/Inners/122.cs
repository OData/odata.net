namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _qualifiedComplexTypeName_ʺx2FʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._qualifiedComplexTypeName_ʺx2Fʺ> Instance { get; } = from _qualifiedComplexTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedComplexTypeNameParser.Instance
from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._qualifiedComplexTypeName_ʺx2Fʺ(_qualifiedComplexTypeName_1, _ʺx2Fʺ_1);
    }
    
}
