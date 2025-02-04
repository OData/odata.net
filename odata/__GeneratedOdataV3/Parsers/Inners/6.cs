namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ> Instance { get; } = (_ʺx68x74x74x70x73ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ>(_ʺx68x74x74x70ʺParser.Instance);
        
        public static class _ʺx68x74x74x70x73ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx68x74x74x70x73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx68x74x74x70x73ʺParser.Instance.Parse(input);
if (!_ʺx68x74x74x70x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70x73ʺ.Instance, _ʺx68x74x74x70x73ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx68x74x74x70ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx68x74x74x70ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx68x74x74x70ʺParser.Instance.Parse(input);
if (!_ʺx68x74x74x70ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ._ʺx68x74x74x70ʺ.Instance, _ʺx68x74x74x70ʺ_1.Remainder);
                }
            }
        }
    }
    
}
