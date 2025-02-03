namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5BʺⳆʺx25x35x42ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ> Instance { get; } = (_ʺx5BʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ>(_ʺx25x35x42ʺParser.Instance);
        
        public static class _ʺx5BʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ> Instance { get; } = from _ʺx5Bʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx5BʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ(_ʺx5Bʺ_1);
        }
        
        public static class _ʺx25x35x42ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ> Instance { get; } = from _ʺx25x35x42ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx25x35x42ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ(_ʺx25x35x42ʺ_1);
        }
    }
    
}
