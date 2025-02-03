namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _lineStringDataParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._lineStringData> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _positionLiteral_1 in __GeneratedOdataV2.Parsers.Rules._positionLiteralParser.Instance
from _ⲤCOMMA_positionLiteralↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤCOMMA_positionLiteralↃParser.Instance.Repeat(1, null)
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._lineStringData(_OPEN_1, _positionLiteral_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ>(_ⲤCOMMA_positionLiteralↃ_1), _CLOSE_1);
    }
    
}
