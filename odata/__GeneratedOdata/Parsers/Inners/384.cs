namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5BʺⳆʺx25x35x42ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ> Instance { get; } = (_ʺx5BʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ>(_ʺx25x35x42ʺParser.Instance);
        
        public static class _ʺx5BʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ> Instance { get; } = from _ʺx5Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5BʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ(_ʺx5Bʺ_1);
        }
        
        public static class _ʺx25x35x42ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ> Instance { get; } = from _ʺx25x35x42ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x35x42ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ(_ʺx25x35x42ʺ_1);
        }
    }
    
}
