namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _countⳆboundOperationParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._countⳆboundOperation> Instance { get; } = (_countParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._countⳆboundOperation>(_boundOperationParser.Instance);
        
        public static class _countParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._countⳆboundOperation._count> Instance { get; } = from _count_1 in __GeneratedOdata.Parsers.Rules._countParser.Instance
select new __GeneratedOdata.CstNodes.Inners._countⳆboundOperation._count(_count_1);
        }
        
        public static class _boundOperationParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._countⳆboundOperation._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdata.CstNodes.Inners._countⳆboundOperation._boundOperation(_boundOperation_1);
        }
    }
    
}
