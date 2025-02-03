namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchWordParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._searchWord> Instance { get; } = from _ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1 in __GeneratedOdataV3.Parsers.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃParser.Instance.Repeat(1, null)
select new __GeneratedOdataV3.CstNodes.Rules._searchWord(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ>(_ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1));
    }
    
}
