namespace __Generated.Parsers.Inners
{
    using _GeneratorV5.ManualParsers.Rules;
    using Sprache;
    
    public static class _ʺx2Dʺ_1ЖHEXDIGParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Dʺ_1ЖHEXDIG> Instance { get; } = from _ʺx2Dʺ_1 in __Generated.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_1 in __Generated.Parsers.Rules._HEXDIGParser.Instance.Many()
select new __Generated.CstNodes.Inners._ʺx2Dʺ_1ЖHEXDIG(_ʺx2Dʺ_1, _HEXDIG_1.Convert2());
    }
    
}
