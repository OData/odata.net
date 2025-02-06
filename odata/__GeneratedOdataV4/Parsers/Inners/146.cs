namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx61x73x63ʺⳆʺx64x65x73x63ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ> Instance { get; } = (_ʺx61x73x63ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ>(_ʺx64x65x73x63ʺParser.Instance);
        
        public static class _ʺx61x73x63ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx61x73x63ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx61x73x63ʺParser.Instance.Parse(input);
if (!_ʺx61x73x63ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ.Instance, _ʺx61x73x63ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx64x65x73x63ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx64x65x73x63ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx64x65x73x63ʺParser.Instance.Parse(input);
if (!_ʺx64x65x73x63ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ.Instance, _ʺx64x65x73x63ʺ_1.Remainder);
                }
            }
        }
    }
    
}
