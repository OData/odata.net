namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx7BʺⳆʺx25x37x42ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ> Instance { get; } = (_ʺx7BʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ>(_ʺx25x37x42ʺParser.Instance);
        
        public static class _ʺx7BʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ> Instance { get; } = from _ʺx7Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx7BʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ(_ʺx7Bʺ_1);
        }
        
        public static class _ʺx25x37x42ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ> Instance { get; } = from _ʺx25x37x42ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x37x42ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ(_ʺx25x37x42ʺ_1);
        }
    }
    
}
