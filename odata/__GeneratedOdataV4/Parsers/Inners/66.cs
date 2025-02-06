namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasⳆprimitiveLiteralParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral> Instance { get; } = (_parameterAliasParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral>(_primitiveLiteralParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias> Parse(IInput<char>? input)
                {
                    var _parameterAlias_1 = __GeneratedOdataV4.Parsers.Rules._parameterAliasParser.Instance.Parse(input);
if (!_parameterAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias(_parameterAlias_1.Parsed), _parameterAlias_1.Remainder);
                }
            }
        }
        
        public static class _primitiveLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral> Parse(IInput<char>? input)
                {
                    var _primitiveLiteral_1 = __GeneratedOdataV4.Parsers.Rules._primitiveLiteralParser.Instance.Parse(input);
if (!_primitiveLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral(_primitiveLiteral_1.Parsed), _primitiveLiteral_1.Remainder);
                }
            }
        }
    }
    
}
