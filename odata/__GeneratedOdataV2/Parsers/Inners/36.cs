namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_qualifiedEntityTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedEntityTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedEntityTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName(_ʺx2Fʺ_1, _qualifiedEntityTypeName_1);
    }
    
}
