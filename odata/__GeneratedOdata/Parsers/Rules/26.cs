namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _ordinalIndexParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._ordinalIndex> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._ordinalIndex(_ʺx2Fʺ_1, _DIGIT_1);
    }
    
}
