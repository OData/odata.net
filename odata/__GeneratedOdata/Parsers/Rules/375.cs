namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _COMMAParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._COMMA> Instance { get; } = (_ʺx2CʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._COMMA>(_ʺx25x32x43ʺParser.Instance);
        
        public static class _ʺx2CʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._COMMA._ʺx2Cʺ> Instance { get; } = from _ʺx2Cʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2CʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._COMMA._ʺx2Cʺ(_ʺx2Cʺ_1);
        }
        
        public static class _ʺx25x32x43ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._COMMA._ʺx25x32x43ʺ> Instance { get; } = from _ʺx25x32x43ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x32x43ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._COMMA._ʺx25x32x43ʺ(_ʺx25x32x43ʺ_1);
        }
    }
    
}
