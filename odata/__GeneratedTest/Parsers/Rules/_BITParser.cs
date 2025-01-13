namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _BITParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._BIT> Instance { get; } = (_ʺx30ʺParser.Instance).Or<__GeneratedTest.CstNodes.Rules._BIT>(_ʺx31ʺParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._BIT._ʺx30ʺ> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedTest.Parsers.Inners._ʺx30ʺParser.Instance
select new __GeneratedTest.CstNodes.Rules._BIT._ʺx30ʺ(_ʺx30ʺ_1);
        }
        
        public static class _ʺx31ʺParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._BIT._ʺx31ʺ> Instance { get; } = from _ʺx31ʺ_1 in __GeneratedTest.Parsers.Inners._ʺx31ʺParser.Instance
select new __GeneratedTest.CstNodes.Rules._BIT._ʺx31ʺ(_ʺx31ʺ_1);
        }
    }
    
}
