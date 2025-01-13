namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx31ʺⳆʺx32ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ> Instance { get; } = (_ʺx31ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ>(_ʺx32ʺParser.Instance);
        
        public static class _ʺx31ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ> Instance { get; } = from _ʺx31ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx31ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ(_ʺx31ʺ_1);
        }
        
        public static class _ʺx32ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ> Instance { get; } = from _ʺx32ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx32ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ(_ʺx32ʺ_1);
        }
    }
    
}
