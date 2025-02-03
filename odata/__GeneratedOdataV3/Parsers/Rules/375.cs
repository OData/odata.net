namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _COMMAParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA> Instance { get; } = (_ʺx2CʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._COMMA>(_ʺx25x32x43ʺParser.Instance);
        
        public static class _ʺx2CʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx2Cʺ> Instance { get; } = from _ʺx2Cʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2CʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx2Cʺ(_ʺx2Cʺ_1);
        }
        
        public static class _ʺx25x32x43ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx25x32x43ʺ> Instance { get; } = from _ʺx25x32x43ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx25x32x43ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx25x32x43ʺ(_ʺx25x32x43ʺ_1);
        }
    }
    
}
