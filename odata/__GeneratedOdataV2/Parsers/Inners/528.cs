namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx31ʺⳆʺx32ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ> Instance { get; } = (_ʺx31ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ>(_ʺx32ʺParser.Instance);
        
        public static class _ʺx31ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ> Instance { get; } = from _ʺx31ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx31ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ(_ʺx31ʺ_1);
        }
        
        public static class _ʺx32ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ> Instance { get; } = from _ʺx32ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx32ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ(_ʺx32ʺ_1);
        }
    }
    
}
