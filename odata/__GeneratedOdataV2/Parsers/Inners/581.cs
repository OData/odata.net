namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _termNameⳆSTARParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._termNameⳆSTAR> Instance { get; } = (_termNameParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._termNameⳆSTAR>(_STARParser.Instance);
        
        public static class _termNameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._termNameⳆSTAR._termName> Instance { get; } = from _termName_1 in __GeneratedOdataV2.Parsers.Rules._termNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._termNameⳆSTAR._termName(_termName_1);
        }
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._termNameⳆSTAR._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV2.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._termNameⳆSTAR._STAR(_STAR_1);
        }
    }
    
}
