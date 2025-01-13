namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Dʺ_1ЖBITParser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._ʺx2Dʺ_1ЖBIT> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedTest.Parsers.Inners._ʺx2DʺParser.Instance
from _BIT_1 in __GeneratedTest.Parsers.Rules._BITParser.Instance.Many()
select new __GeneratedTest.CstNodes.Inners._ʺx2Dʺ_1ЖBIT(_ʺx2Dʺ_1, _BIT_1);
    }
    
}
