namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _BITParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._BIT> Instance { get; } = (_ʺx30ʺParser.Instance).Or<char, __Generated.CstNodes.Rules._BIT>(_ʺx31ʺParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._BIT._ʺx30ʺ> Instance { get; } = from _ʺx30ʺ_1 in __Generated.Parsers.Inners._ʺx30ʺParser.Instance
select new __Generated.CstNodes.Rules._BIT._ʺx30ʺ(_ʺx30ʺ_1);
        }
        
        public static class _ʺx31ʺParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._BIT._ʺx31ʺ> Instance { get; } = from _ʺx31ʺ_1 in __Generated.Parsers.Inners._ʺx31ʺParser.Instance
select new __Generated.CstNodes.Rules._BIT._ʺx31ʺ(_ʺx31ʺ_1);
        }
    }
    
}
