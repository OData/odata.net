namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _countⳆboundOperationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation> Instance { get; } = (_countParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation>(_boundOperationParser.Instance);
        
        public static class _countParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count> Parse(IInput<char>? input)
                {
                    var _count_1 = __GeneratedOdataV3.Parsers.Rules._countParser.Instance.Parse(input);
if (!_count_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._count(_count_1.Parsed), _count_1.Remainder);
                }
            }
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation> Parse(IInput<char>? input)
                {
                    var _boundOperation_1 = __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance.Parse(input);
if (!_boundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._countⳆboundOperation._boundOperation(_boundOperation_1.Parsed), _boundOperation_1.Remainder);
                }
            }
        }
    }
    
}
