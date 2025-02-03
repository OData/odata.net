namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _lineStringData_ЖⲤCOMMA_lineStringDataↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ> Instance { get; } = from _lineStringData_1 in __GeneratedOdataV2.Parsers.Rules._lineStringDataParser.Instance
from _ⲤCOMMA_lineStringDataↃ_1 in Inners._ⲤCOMMA_lineStringDataↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ(_lineStringData_1, _ⲤCOMMA_lineStringDataↃ_1);
    }
    
}
