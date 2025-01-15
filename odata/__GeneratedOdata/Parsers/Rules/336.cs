namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _ringLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._ringLiteral> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _positionLiteral_1 in __GeneratedOdata.Parsers.Rules._positionLiteralParser.Instance
from _ⲤCOMMA_positionLiteralↃ_1 in Inners._ⲤCOMMA_positionLiteralↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._ringLiteral(_OPEN_1, _positionLiteral_1, _ⲤCOMMA_positionLiteralↃ_1, _CLOSE_1);
    }
    
}
