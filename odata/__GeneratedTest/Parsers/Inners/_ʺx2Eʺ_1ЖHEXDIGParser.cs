namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Eʺ_1ЖHEXDIGParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Eʺ_1ЖHEXDIG> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedTest.Parsers.Inners._ʺx2EʺParser.Instance
from _HEXDIG_1 in __GeneratedTest.Parsers.Rules._HEXDIGParser.Instance.Many()
select new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖHEXDIG(_ʺx2Eʺ_1, _HEXDIG_1);
    }
    
}
