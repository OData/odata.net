namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ> Instance { get; } = (_SPParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ>(_HTABParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ>(_ʺx25x32x30ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ>(_ʺx25x30x39ʺParser.Instance);
        
        public static class _SPParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP> Instance { get; } = from _SP_1 in __GeneratedOdata.Parsers.Rules._SPParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP(_SP_1);
        }
        
        public static class _HTABParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB> Instance { get; } = from _HTAB_1 in __GeneratedOdata.Parsers.Rules._HTABParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB(_HTAB_1);
        }
        
        public static class _ʺx25x32x30ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ> Instance { get; } = from _ʺx25x32x30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x32x30ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ(_ʺx25x32x30ʺ_1);
        }
        
        public static class _ʺx25x30x39ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ> Instance { get; } = from _ʺx25x30x39ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x30x39ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ(_ʺx25x30x39ʺ_1);
        }
    }
    
}