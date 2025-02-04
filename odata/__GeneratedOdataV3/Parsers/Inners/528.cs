namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx31ʺⳆʺx32ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ> Instance { get; } = (_ʺx31ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ>(_ʺx32ʺParser.Instance);
        
        public static class _ʺx31ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx31ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx31ʺParser.Instance.Parse(input);
if (!_ʺx31ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx31ʺ(_ʺx31ʺ_1.Parsed), _ʺx31ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx32ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx32ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx32ʺParser.Instance.Parse(input);
if (!_ʺx32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺⳆʺx32ʺ._ʺx32ʺ(_ʺx32ʺ_1.Parsed), _ʺx32ʺ_1.Remainder);
                }
            }
        }
    }
    
}
