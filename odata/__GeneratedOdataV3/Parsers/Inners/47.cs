namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _countⳆboundOperationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation> Instance { get; } = (_countParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation>(_boundOperationParser.Instance);
        
        public static class _countParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count> Instance { get; } = from _count_1 in __GeneratedOdataV3.Parsers.Rules._countParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count(_count_1);
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation(_boundOperation_1);
        }
    }
    
}
