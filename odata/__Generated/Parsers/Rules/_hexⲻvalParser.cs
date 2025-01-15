namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _hexⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._hexⲻval> Instance { get; } = from _ʺx78ʺ_1 in __Generated.Parsers.Inners._ʺx78ʺParser.Instance
from _HEXDIG_1 in __Generated.Parsers.Rules._HEXDIGParser.Instance.Many()
from _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 in __Generated.Parsers.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃParser.Instance.Optional()
select new __Generated.CstNodes.Rules._hexⲻval(_ʺx78ʺ_1, new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Rules._HEXDIG>(_HEXDIG_1), _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1.GetOrElse(null));
    }
    
}
