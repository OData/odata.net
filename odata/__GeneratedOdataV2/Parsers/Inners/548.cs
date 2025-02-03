namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _pointData_ЖⲤCOMMA_pointDataↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ> Instance { get; } = from _pointData_1 in __GeneratedOdataV2.Parsers.Rules._pointDataParser.Instance
from _ⲤCOMMA_pointDataↃ_1 in Inners._ⲤCOMMA_pointDataↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ(_pointData_1, _ⲤCOMMA_pointDataↃ_1);
    }
    
}
