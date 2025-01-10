namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _commentParser
    {
        public static Parser<__Generated.CstNodes.Rules._comment> Instance { get; } = from _ʺx3Bʺ_1 in __Generated.Parsers.Inners._ʺx3BʺParser.Instance
from _ⲤWSPⳆVCHARↃ_1 in __Generated.Parsers.Inners._ⲤWSPⳆVCHARↃParser.Instance.Many()
from _CRLF_1 in __Generated.Parsers.Rules._CRLFParser.Instance
select new __Generated.CstNodes.Rules._comment(_ʺx3Bʺ_1, _ⲤWSPⳆVCHARↃ_1, _CRLF_1);
    }
    
}
