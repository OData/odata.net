namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _binⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._binⲻval> Instance { get; } = from _ʺx62ʺ_1 in __GeneratedTest.Parsers.Inners._ʺx62ʺParser.Instance
from _BIT_1 in __GeneratedTest.Parsers.Rules._BITParser.Instance.AtLeastOnce()
from _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1 in __GeneratedTest.Parsers.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃParser.Instance.Optional()
select new __Generated.CstNodes.Rules._binⲻval(_ʺx62ʺ_1, _BIT_1, _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1.GetOrElse(null));
    }
    
}
