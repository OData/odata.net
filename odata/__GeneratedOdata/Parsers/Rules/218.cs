namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _escapeParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._escape> Instance { get; } = (_ʺx5CʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._escape>(_ʺx25x35x43ʺParser.Instance);
        
        public static class _ʺx5CʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._escape._ʺx5Cʺ> Instance { get; } = from _ʺx5Cʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5CʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._escape._ʺx5Cʺ(_ʺx5Cʺ_1);
        }
        
        public static class _ʺx25x35x43ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._escape._ʺx25x35x43ʺ> Instance { get; } = from _ʺx25x35x43ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x35x43ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._escape._ʺx25x35x43ʺ(_ʺx25x35x43ʺ_1);
        }
    }
    
}
