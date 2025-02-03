namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _polygonDataParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._polygonData> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _ringLiteral_1 in __GeneratedOdataV2.Parsers.Rules._ringLiteralParser.Instance
from _ⲤCOMMA_ringLiteralↃ_1 in Inners._ⲤCOMMA_ringLiteralↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._polygonData(_OPEN_1, _ringLiteral_1, _ⲤCOMMA_ringLiteralↃ_1, _CLOSE_1);
    }
    
}
