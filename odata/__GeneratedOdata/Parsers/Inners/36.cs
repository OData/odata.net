namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_qualifiedEntityTypeNameParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedEntityTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName(_ʺx2Fʺ_1, _qualifiedEntityTypeName_1);
    }
    
}
