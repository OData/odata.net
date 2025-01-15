namespace __Generated.Parsers.Inners
{
    using _GeneratorV5.ManualParsers.Rules;
    using Sprache;
    
    public static class _ʺx2Dʺ_1ЖBITParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Dʺ_1ЖBIT> Instance { get; } = from _ʺx2Dʺ_1 in __Generated.Parsers.Inners._ʺx2DʺParser.Instance
from _BIT_1 in __Generated.Parsers.Rules._BITParser.Instance.Many()
select new __Generated.CstNodes.Inners._ʺx2Dʺ_1ЖBIT(_ʺx2Dʺ_1, _BIT_1.Convert2());
    }
    
}
