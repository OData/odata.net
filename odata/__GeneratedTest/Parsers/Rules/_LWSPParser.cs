namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _LWSPParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._LWSP> Instance { get; } = from _ⲤWSPⳆCRLF_WSPↃ_1 in __GeneratedTest.Parsers.Inners._ⲤWSPⳆCRLF_WSPↃParser.Instance.Many()
select new __GeneratedTest.CstNodes.Rules._LWSP(_ⲤWSPⳆCRLF_WSPↃ_1);
    }
    
}
