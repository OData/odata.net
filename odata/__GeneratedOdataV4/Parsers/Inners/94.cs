namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ> Instance { get; } = (_ʺx24x63x6Fx6Dx70x75x74x65ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ>(_ʺx63x6Fx6Dx70x75x74x65ʺParser.Instance);
        
        public static class _ʺx24x63x6Fx6Dx70x75x74x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx24x63x6Fx6Dx70x75x74x65ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx24x63x6Fx6Dx70x75x74x65ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx24x63x6Fx6Dx70x75x74x65ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x63x6Fx6Dx70x75x74x65ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺParser.Instance.Parse(input);
if (!_ʺx24x63x6Fx6Dx70x75x74x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx24x63x6Fx6Dx70x75x74x65ʺ.Instance, _ʺx24x63x6Fx6Dx70x75x74x65ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx63x6Fx6Dx70x75x74x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx63x6Fx6Dx70x75x74x65ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx63x6Fx6Dx70x75x74x65ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx63x6Fx6Dx70x75x74x65ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx63x6Fx6Dx70x75x74x65ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx63x6Fx6Dx70x75x74x65ʺParser.Instance.Parse(input);
if (!_ʺx63x6Fx6Dx70x75x74x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx63x6Fx6Dx70x75x74x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺ._ʺx63x6Fx6Dx70x75x74x65ʺ.Instance, _ʺx63x6Fx6Dx70x75x74x65ʺ_1.Remainder);
                }
            }
        }
    }
    
}
