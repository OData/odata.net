namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _nanInfinityParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._nanInfinity> Instance { get; } = (_ʺx4Ex61x4EʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._nanInfinity>(_ʺx2Dx49x4Ex46ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._nanInfinity>(_ʺx49x4Ex46ʺParser.Instance);
        
        public static class _ʺx4Ex61x4EʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ> Instance { get; } = from _ʺx4Ex61x4Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4Ex61x4EʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ(_ʺx4Ex61x4Eʺ_1);
        }
        
        public static class _ʺx2Dx49x4Ex46ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ> Instance { get; } = from _ʺx2Dx49x4Ex46ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2Dx49x4Ex46ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ(_ʺx2Dx49x4Ex46ʺ_1);
        }
        
        public static class _ʺx49x4Ex46ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ> Instance { get; } = from _ʺx49x4Ex46ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx49x4Ex46ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ(_ʺx49x4Ex46ʺ_1);
        }
    }
    
}
