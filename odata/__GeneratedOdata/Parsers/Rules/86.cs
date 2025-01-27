namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchWordParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._searchWord> Instance { get; } = from _ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._searchWord(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ>(_ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1));
    }
    
}
