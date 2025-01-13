namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationParser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation> Instance { get; } = from _cⲻwsp_1 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _ʺx2Fʺ_1 in __GeneratedTest.Parsers.Inners._ʺx2FʺParser.Instance
from _cⲻwsp_2 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _concatenation_1 in __GeneratedTest.Parsers.Rules._concatenationParser.Instance
select new __GeneratedTest.CstNodes.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation(_cⲻwsp_1, _ʺx2Fʺ_1, _cⲻwsp_2, _concatenation_1);
    }
    
}
