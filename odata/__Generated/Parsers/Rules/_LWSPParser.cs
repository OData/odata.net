namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _LWSPParser
    {
        public static Parser<__Generated.CstNodes.Rules._LWSP> Instance { get; } = from _ⲤWSPⳆCRLF_WSPↃ_1 in __Generated.Parsers.Inners._ⲤWSPⳆCRLF_WSPↃParser.Instance.Many()
select new __Generated.CstNodes.Rules._LWSP(_ⲤWSPⳆCRLF_WSPↃ_1);
    }
    
}
