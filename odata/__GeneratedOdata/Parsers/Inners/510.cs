namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _1ЖDIGIT_ʺx4DʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
from _ʺx4Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4DʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ(_DIGIT_1, _ʺx4Dʺ_1);
    }
    
}