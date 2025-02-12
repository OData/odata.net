namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2FʺⳆʺx25x32x46ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ> Instance { get; } = (_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ>(_ʺx25x32x46ʺParser.Instance);
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ.Instance;
        }
        
        public static class _ʺx25x32x46ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ> Instance { get; } = from _ʺx25x32x46ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x32x46ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ.Instance;
        }
    }
    
}
