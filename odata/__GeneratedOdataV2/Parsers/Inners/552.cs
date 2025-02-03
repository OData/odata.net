namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _polygonData_ЖⲤCOMMA_polygonDataↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ> Instance { get; } = from _polygonData_1 in __GeneratedOdataV2.Parsers.Rules._polygonDataParser.Instance
from _ⲤCOMMA_polygonDataↃ_1 in Inners._ⲤCOMMA_polygonDataↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ(_polygonData_1, _ⲤCOMMA_polygonDataↃ_1);
    }
    
}
