namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2DʺⳆʺx2BʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ> Instance { get; } = (_ʺx2DʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ>(_ʺx2BʺParser.Instance);
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ._ʺx2Dʺ> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ._ʺx2Dʺ(_ʺx2Dʺ_1);
        }
        
        public static class _ʺx2BʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ._ʺx2Bʺ> Instance { get; } = from _ʺx2Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2BʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2DʺⳆʺx2Bʺ._ʺx2Bʺ(_ʺx2Bʺ_1);
        }
    }
    
}
