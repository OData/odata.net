namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _ЖDIGIT_ʺx2Aʺ_ЖDIGITParser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._ЖDIGIT_ʺx2Aʺ_ЖDIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedTest.Parsers.Rules._DIGITParser.Instance.Many()
from _ʺx2Aʺ_1 in __GeneratedTest.Parsers.Inners._ʺx2AʺParser.Instance
from _DIGIT_2 in __GeneratedTest.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedTest.CstNodes.Inners._ЖDIGIT_ʺx2Aʺ_ЖDIGIT(_DIGIT_1, _ʺx2Aʺ_1, _DIGIT_2);
    }
    
}
