namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ> Instance { get; } = (_ʺx30ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ>(_ʺx31ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ>(_ʺx32ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ>(_ʺx33ʺParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx30ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx30ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx30ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx30ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx30ʺParser.Instance.Parse(input);
if (!_ʺx30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx30ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx30ʺ.Instance, _ʺx30ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx31ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx31ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx31ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx31ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx31ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx31ʺParser.Instance.Parse(input);
if (!_ʺx31ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx31ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx31ʺ.Instance, _ʺx31ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx32ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx32ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx32ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx32ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx32ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx32ʺParser.Instance.Parse(input);
if (!_ʺx32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx32ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx32ʺ.Instance, _ʺx32ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx33ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx33ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx33ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx33ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx33ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx33ʺParser.Instance.Parse(input);
if (!_ʺx33ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx33ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ._ʺx33ʺ.Instance, _ʺx33ʺ_1.Remainder);
                }
            }
        }
    }
    
}
