namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _STARParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._STAR> Instance { get; } = (_ʺx2AʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._STAR>(_ʺx25x32x41ʺParser.Instance);
        
        public static class _ʺx2AʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._STAR._ʺx2Aʺ> Instance { get; } = from _ʺx2Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2AʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._STAR._ʺx2Aʺ(_ʺx2Aʺ_1);
        }
        
        public static class _ʺx25x32x41ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._STAR._ʺx25x32x41ʺ> Instance { get; } = from _ʺx25x32x41ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x32x41ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._STAR._ʺx25x32x41ʺ(_ʺx25x32x41ʺ_1);
        }
    }
    
}
