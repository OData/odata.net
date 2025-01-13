namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _hexⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._hexⲻval> Instance { get; } = from _ʺx78ʺ_1 in __GeneratedTest.Parsers.Inners._ʺx78ʺParser.Instance
from _HEXDIG_1 in __GeneratedTest.Parsers.Rules._HEXDIGParser.Instance.AtLeastOnce()
from _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 in __GeneratedTest.Parsers.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃParser.Instance.Optional()
select new __Generated.CstNodes.Rules._hexⲻval(_ʺx78ʺ_1, _HEXDIG_1, _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1.GetOrElse(null));
    }
    
}
