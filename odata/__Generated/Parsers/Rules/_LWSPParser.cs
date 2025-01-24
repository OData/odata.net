namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _LWSPParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._LWSP> Instance { get; } = from _ⲤWSPⳆCRLF_WSPↃ_1 in Inners._ⲤWSPⳆCRLF_WSPↃParser.Instance.Many()
select new __Generated.CstNodes.Rules._LWSP(_ⲤWSPⳆCRLF_WSPↃ_1);
    }
    
}
