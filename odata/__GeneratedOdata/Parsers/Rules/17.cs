namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _primitiveColPathParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveColPath> Instance { get; } = (_countParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveColPath>(_boundOperationParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveColPath>(_ordinalIndexParser.Instance);
        
        public static class _countParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveColPath._count> Instance { get; } = from _count_1 in __GeneratedOdata.Parsers.Rules._countParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveColPath._count(_count_1);
        }
        
        public static class _boundOperationParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveColPath._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveColPath._boundOperation(_boundOperation_1);
        }
        
        public static class _ordinalIndexParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveColPath._ordinalIndex> Instance { get; } = from _ordinalIndex_1 in __GeneratedOdata.Parsers.Rules._ordinalIndexParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveColPath._ordinalIndex(_ordinalIndex_1);
        }
    }
    
}
