namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ> Instance { get; } = (_ʺx24x69x6Ex64x65x78ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ>(_ʺx69x6Ex64x65x78ʺParser.Instance);
        
        public static class _ʺx24x69x6Ex64x65x78ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx24x69x6Ex64x65x78ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx24x69x6Ex64x65x78ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx24x69x6Ex64x65x78ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x69x6Ex64x65x78ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x69x6Ex64x65x78ʺParser.Instance.Parse(input);
if (!_ʺx24x69x6Ex64x65x78ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx24x69x6Ex64x65x78ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx24x69x6Ex64x65x78ʺ.Instance, _ʺx24x69x6Ex64x65x78ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx69x6Ex64x65x78ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx69x6Ex64x65x78ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx69x6Ex64x65x78ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx69x6Ex64x65x78ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx69x6Ex64x65x78ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx69x6Ex64x65x78ʺParser.Instance.Parse(input);
if (!_ʺx69x6Ex64x65x78ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx69x6Ex64x65x78ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx69x6Ex64x65x78ʺ.Instance, _ʺx69x6Ex64x65x78ʺ_1.Remainder);
                }
            }
        }
    }
    
}
