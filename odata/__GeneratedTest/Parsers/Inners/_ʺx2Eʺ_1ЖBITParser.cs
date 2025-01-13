namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Eʺ_1ЖBITParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Eʺ_1ЖBIT> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedTest.Parsers.Inners._ʺx2EʺParser.Instance
from _BIT_1 in __GeneratedTest.Parsers.Rules._BITParser.Instance.AtLeastOnce()
select new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖBIT(_ʺx2Eʺ_1, _BIT_1);
    }
    
}
