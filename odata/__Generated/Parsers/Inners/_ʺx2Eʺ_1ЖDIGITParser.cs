namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Eʺ_1ЖDIGITParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT> Instance { get; } = from _ʺx2Eʺ_1 in __Generated.Parsers.Inners._ʺx2EʺParser.Instance
from _DIGIT_1 in __Generated.Parsers.Rules._DIGITParser.Instance.Many()
select new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT(_ʺx2Eʺ_1, _DIGIT_1);
    }
    
}
