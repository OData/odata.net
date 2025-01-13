namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _multiPolygonLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._multiPolygonLiteral> Instance { get; } = from _ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺParser.Instance
from _polygonData_ЖⲤCOMMA_polygonDataↃ_1 in __GeneratedOdata.Parsers.Inners._polygonData_ЖⲤCOMMA_polygonDataↃParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._multiPolygonLiteral(_ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1, _polygonData_ЖⲤCOMMA_polygonDataↃ_1.GetOrElse(null), _CLOSE_1);
    }
    
}
