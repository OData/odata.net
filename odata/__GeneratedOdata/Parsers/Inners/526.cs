namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx30ʺⳆʺx31ʺⳆʺx32ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ> Instance { get; } = (_ʺx30ʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ>(_ʺx31ʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ>(_ʺx32ʺParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx30ʺ> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx30ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx30ʺ(_ʺx30ʺ_1);
        }
        
        public static class _ʺx31ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx31ʺ> Instance { get; } = from _ʺx31ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx31ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx31ʺ(_ʺx31ʺ_1);
        }
        
        public static class _ʺx32ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx32ʺ> Instance { get; } = from _ʺx32ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx32ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺ._ʺx32ʺ(_ʺx32ʺ_1);
        }
    }
    
}
