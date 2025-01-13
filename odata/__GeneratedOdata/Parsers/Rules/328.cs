namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _sridLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._sridLiteral> Instance { get; } = from _ʺx53x52x49x44ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx53x52x49x44ʺParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
from _SEMI_1 in __GeneratedOdata.Parsers.Rules._SEMIParser.Instance
select new __GeneratedOdata.CstNodes.Rules._sridLiteral(_ʺx53x52x49x44ʺ_1, _EQ_1, _DIGIT_1, _SEMI_1);
    }
    
}
