namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx30ʺⳆʺx31ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ> Instance { get; } = (_ʺx30ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ>(_ʺx31ʺParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx30ʺ> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx30ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx30ʺ(_ʺx30ʺ_1);
        }
        
        public static class _ʺx31ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx31ʺ> Instance { get; } = from _ʺx31ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx31ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx31ʺ(_ʺx31ʺ_1);
        }
    }
    
}
