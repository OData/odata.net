namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _pointData_ЖⲤCOMMA_pointDataↃParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ> Instance { get; } = from _pointData_1 in __GeneratedOdata.Parsers.Rules._pointDataParser.Instance
from _ⲤCOMMA_pointDataↃ_1 in Inners._ⲤCOMMA_pointDataↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ(_pointData_1, _ⲤCOMMA_pointDataↃ_1);
    }
    
}
