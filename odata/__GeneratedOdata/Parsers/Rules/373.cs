namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _ATParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._AT> Instance { get; } = (_ʺx40ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._AT>(_ʺx25x34x30ʺParser.Instance);
        
        public static class _ʺx40ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._AT._ʺx40ʺ> Instance { get; } = from _ʺx40ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._AT._ʺx40ʺ(_ʺx40ʺ_1);
        }
        
        public static class _ʺx25x34x30ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._AT._ʺx25x34x30ʺ> Instance { get; } = from _ʺx25x34x30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25x34x30ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._AT._ʺx25x34x30ʺ(_ʺx25x34x30ʺ_1);
        }
    }
    
}
