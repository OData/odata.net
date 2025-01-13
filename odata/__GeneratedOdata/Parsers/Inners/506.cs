namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _1ЖDIGIT_ʺx44ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx44ʺ> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
from _ʺx44ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx44ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx44ʺ(_DIGIT_1, _ʺx44ʺ_1);
    }
    
}
