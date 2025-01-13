namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _polygonDataParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._polygonData> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _ringLiteral_1 in __GeneratedOdata.Parsers.Rules._ringLiteralParser.Instance
from _ⲤCOMMA_ringLiteralↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_ringLiteralↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._polygonData(_OPEN_1, _ringLiteral_1, _ⲤCOMMA_ringLiteralↃ_1, _CLOSE_1);
    }
    
}
