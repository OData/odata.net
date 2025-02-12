namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5DʺⳆʺx25x35x44ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ> Instance { get; } = (_ʺx5DʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ>(_ʺx25x35x44ʺParser.Instance);
        
        public static class _ʺx5DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ> Instance { get; } = from _ʺx5Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5DʺParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ.Instance;
        }
        
        public static class _ʺx25x35x44ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ> Instance { get; } = from _ʺx25x35x44ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x35x44ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ.Instance;
        }
    }
    
}
