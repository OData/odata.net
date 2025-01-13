namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _STARParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._STAR> Instance { get; } = (_ʺx2AʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._STAR>(_ʺx25x32x41ʺParser.Instance);
        
        public static class _ʺx2AʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._STAR._ʺx2Aʺ> Instance { get; } = from _ʺx2Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2AʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._STAR._ʺx2Aʺ(_ʺx2Aʺ_1);
        }
        
        public static class _ʺx25x32x41ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._STAR._ʺx25x32x41ʺ> Instance { get; } = from _ʺx25x32x41ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x32x41ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._STAR._ʺx25x32x41ʺ(_ʺx25x32x41ʺ_1);
        }
    }
    
}
