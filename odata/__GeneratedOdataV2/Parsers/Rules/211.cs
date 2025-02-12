namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _quotationⲻmarkParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._quotationⲻmark> Instance { get; } = (_DQUOTEParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._quotationⲻmark>(_ʺx25x32x32ʺParser.Instance);
        
        public static class _DQUOTEParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._quotationⲻmark._DQUOTE> Instance { get; } = from _DQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._DQUOTEParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._quotationⲻmark._DQUOTE.Instance;
        }
        
        public static class _ʺx25x32x32ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ> Instance { get; } = from _ʺx25x32x32ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x32x32ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ.Instance;
        }
    }
    
}
