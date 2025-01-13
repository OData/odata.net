namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _SQUOTEParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._SQUOTE> Instance { get; } = (_ʺx27ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._SQUOTE>(_ʺx25x32x37ʺParser.Instance);
        
        public static class _ʺx27ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._SQUOTE._ʺx27ʺ> Instance { get; } = from _ʺx27ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx27ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SQUOTE._ʺx27ʺ(_ʺx27ʺ_1);
        }
        
        public static class _ʺx25x32x37ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ> Instance { get; } = from _ʺx25x32x37ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x32x37ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ(_ʺx25x32x37ʺ_1);
        }
    }
    
}
