namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SIGNParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._SIGN> Instance { get; } = (_ʺx2BʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._SIGN>(_ʺx25x32x42ʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._SIGN>(_ʺx2DʺParser.Instance);
        
        public static class _ʺx2BʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._SIGN._ʺx2Bʺ> Instance { get; } = from _ʺx2Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2BʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SIGN._ʺx2Bʺ(_ʺx2Bʺ_1);
        }
        
        public static class _ʺx25x32x42ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._SIGN._ʺx25x32x42ʺ> Instance { get; } = from _ʺx25x32x42ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x32x42ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SIGN._ʺx25x32x42ʺ(_ʺx25x32x42ʺ_1);
        }
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._SIGN._ʺx2Dʺ> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SIGN._ʺx2Dʺ(_ʺx2Dʺ_1);
        }
    }
    
}
