namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _1ЖDIGIT_ʺx48ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
from _ʺx48ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx48ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ(_DIGIT_1, _ʺx48ʺ_1);
    }
    
}