namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SPⳆHTABParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SPⳆHTAB> Instance { get; } = (_SPParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._SPⳆHTAB>(_HTABParser.Instance);
        
        public static class _SPParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SPⳆHTAB._SP> Instance { get; } = from _SP_1 in __GeneratedOdataV2.Parsers.Rules._SPParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._SPⳆHTAB._SP.Instance;
        }
        
        public static class _HTABParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SPⳆHTAB._HTAB> Instance { get; } = from _HTAB_1 in __GeneratedOdataV2.Parsers.Rules._HTABParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._SPⳆHTAB._HTAB.Instance;
        }
    }
    
}
