namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _SEMIParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._SEMI> Instance { get; } = (_ʺx3BʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._SEMI>(_ʺx25x33x42ʺParser.Instance);
        
        public static class _ʺx3BʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._SEMI._ʺx3Bʺ> Instance { get; } = from _ʺx3Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3BʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SEMI._ʺx3Bʺ(_ʺx3Bʺ_1);
        }
        
        public static class _ʺx25x33x42ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._SEMI._ʺx25x33x42ʺ> Instance { get; } = from _ʺx25x33x42ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x33x42ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SEMI._ʺx25x33x42ʺ(_ʺx25x33x42ʺ_1);
        }
    }
    
}
