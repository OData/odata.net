namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _fracParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._frac> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._frac(_ʺx2Eʺ_1, _DIGIT_1);
    }
    
}