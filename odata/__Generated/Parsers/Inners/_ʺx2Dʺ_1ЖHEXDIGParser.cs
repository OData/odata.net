namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Dʺ_1ЖHEXDIGParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Dʺ_1ЖHEXDIG> Instance { get; } = from _ʺx2Dʺ_1 in __Generated.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_1 in __Generated.Parsers.Rules._HEXDIGParser.Instance.Repeat(1, null)
select new __Generated.CstNodes.Inners._ʺx2Dʺ_1ЖHEXDIG(_ʺx2Dʺ_1, new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Rules._HEXDIG>(_HEXDIG_1));
    }
    
}
