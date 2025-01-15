namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Eʺ_1ЖHEXDIGParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Eʺ_1ЖHEXDIG> Instance { get; } = from _ʺx2Eʺ_1 in __Generated.Parsers.Inners._ʺx2EʺParser.Instance
from _HEXDIG_1 in __Generated.Parsers.Rules._HEXDIGParser.Instance.Many()
select new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖHEXDIG(_ʺx2Eʺ_1, new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Rules._HEXDIG>(_HEXDIG_1));
    }
    
}
