namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx30ʺⳆʺx31ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ> Instance { get; } = (_ʺx30ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ>(_ʺx31ʺParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx30ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx30ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx30ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx30ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx30ʺParser.Instance.Parse(input);
if (!_ʺx30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx30ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx30ʺ.Instance, _ʺx30ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx31ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx31ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx31ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx31ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx31ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx31ʺParser.Instance.Parse(input);
if (!_ʺx31ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx31ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx30ʺⳆʺx31ʺ._ʺx31ʺ.Instance, _ʺx31ʺ_1.Remainder);
                }
            }
        }
    }
    
}
