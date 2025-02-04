namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ> Instance { get; } = (_ʺx24x73x6Bx69x70ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ>(_ʺx73x6Bx69x70ʺParser.Instance);
        
        public static class _ʺx24x73x6Bx69x70ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx24x73x6Bx69x70ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx24x73x6Bx69x70ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx24x73x6Bx69x70ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x73x6Bx69x70ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x73x6Bx69x70ʺParser.Instance.Parse(input);
if (!_ʺx24x73x6Bx69x70ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx24x73x6Bx69x70ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx24x73x6Bx69x70ʺ(_ʺx24x73x6Bx69x70ʺ_1.Parsed), _ʺx24x73x6Bx69x70ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx73x6Bx69x70ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx73x6Bx69x70ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx73x6Bx69x70ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx73x6Bx69x70ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx73x6Bx69x70ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx73x6Bx69x70ʺParser.Instance.Parse(input);
if (!_ʺx73x6Bx69x70ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx73x6Bx69x70ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx73x6Bx69x70ʺ(_ʺx73x6Bx69x70ʺ_1.Parsed), _ʺx73x6Bx69x70ʺ_1.Remainder);
                }
            }
        }
    }
    
}
