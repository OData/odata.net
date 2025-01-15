namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _lineStringDataParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._lineStringData> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _positionLiteral_1 in __GeneratedOdata.Parsers.Rules._positionLiteralParser.Instance
from _ⲤCOMMA_positionLiteralↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_positionLiteralↃParser.Instance.Repeat(1, null)
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._lineStringData(_OPEN_1, _positionLiteral_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ>(_ⲤCOMMA_positionLiteralↃ_1), _CLOSE_1);
    }
    
}
