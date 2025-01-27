namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _COLONParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._COLON> Instance { get; } = (_ʺx3AʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._COLON>(_ʺx25x33x41ʺParser.Instance);
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._COLON._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._COLON._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
        
        public static class _ʺx25x33x41ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._COLON._ʺx25x33x41ʺ> Instance { get; } = from _ʺx25x33x41ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x33x41ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._COLON._ʺx25x33x41ʺ(_ʺx25x33x41ʺ_1);
        }
    }
    
}
