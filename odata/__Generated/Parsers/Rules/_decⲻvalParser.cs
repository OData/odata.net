namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _decⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._decⲻval> Instance { get; } = from _ʺx64ʺ_1 in __Generated.Parsers.Inners._ʺx64ʺParser.Instance
from _DIGIT_1 in __Generated.Parsers.Rules._DIGITParser.Instance.Many()
from _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1 in __Generated.Parsers.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃParser.Instance.Optional()
select new __Generated.CstNodes.Rules._decⲻval(_ʺx64ʺ_1, new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Rules._DIGIT>(_DIGIT_1), _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1.GetOrElse(null));
    }
    
}
