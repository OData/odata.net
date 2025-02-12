namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx7DʺⳆʺx25x37x44ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ> Instance { get; } = (_ʺx7DʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ>(_ʺx25x37x44ʺParser.Instance);
        
        public static class _ʺx7DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ> Instance { get; } = from _ʺx7Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx7DʺParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ.Instance;
        }
        
        public static class _ʺx25x37x44ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ> Instance { get; } = from _ʺx25x37x44ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x37x44ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ.Instance;
        }
    }
    
}
