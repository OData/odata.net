namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColPathParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColPath> Instance { get; } = (_countParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColPath>(_boundOperationParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColPath>(_ordinalIndexParser.Instance);
        
        public static class _countParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColPath._count> Instance { get; } = from _count_1 in __GeneratedOdataV2.Parsers.Rules._countParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._primitiveColPath._count.Instance;
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColPath._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdataV2.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveColPath._boundOperation(_boundOperation_1);
        }
        
        public static class _ordinalIndexParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColPath._ordinalIndex> Instance { get; } = from _ordinalIndex_1 in __GeneratedOdataV2.Parsers.Rules._ordinalIndexParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveColPath._ordinalIndex(_ordinalIndex_1);
        }
    }
    
}
