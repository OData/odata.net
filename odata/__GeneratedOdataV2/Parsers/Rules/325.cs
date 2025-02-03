namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _multiPolygonLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._multiPolygonLiteral> Instance { get; } = from _ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺParser.Instance
from _polygonData_ЖⲤCOMMA_polygonDataↃ_1 in __GeneratedOdataV2.Parsers.Inners._polygonData_ЖⲤCOMMA_polygonDataↃParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._multiPolygonLiteral(_ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1, _polygonData_ЖⲤCOMMA_polygonDataↃ_1.GetOrElse(null), _CLOSE_1);
    }
    
}
