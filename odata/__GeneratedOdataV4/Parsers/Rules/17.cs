namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColPathParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath> Instance { get; } = (_countParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath>(_boundOperationParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath>(_ordinalIndexParser.Instance);
        
        public static class _countParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._count> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._count>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._count> Parse(IInput<char>? input)
                {
                    var _count_1 = __GeneratedOdataV4.Parsers.Rules._countParser.Instance.Parse(input);
if (!_count_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveColPath._count)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._count.Instance, _count_1.Remainder);
                }
            }
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._boundOperation> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._boundOperation>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._boundOperation> Parse(IInput<char>? input)
                {
                    var _boundOperation_1 = __GeneratedOdataV4.Parsers.Rules._boundOperationParser.Instance.Parse(input);
if (!_boundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveColPath._boundOperation)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._boundOperation(_boundOperation_1.Parsed), _boundOperation_1.Remainder);
                }
            }
        }
        
        public static class _ordinalIndexParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._ordinalIndex> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._ordinalIndex>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._ordinalIndex> Parse(IInput<char>? input)
                {
                    var _ordinalIndex_1 = __GeneratedOdataV4.Parsers.Rules._ordinalIndexParser.Instance.Parse(input);
if (!_ordinalIndex_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveColPath._ordinalIndex)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveColPath._ordinalIndex(_ordinalIndex_1.Parsed), _ordinalIndex_1.Remainder);
                }
            }
        }
    }
    
}
