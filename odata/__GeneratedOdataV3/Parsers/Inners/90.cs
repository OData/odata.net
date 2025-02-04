namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x69x64ʺⳆʺx69x64ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ> Instance { get; } = (_ʺx24x69x64ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ>(_ʺx69x64ʺParser.Instance);
        
        public static class _ʺx24x69x64ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx24x69x64ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx24x69x64ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx24x69x64ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x69x64ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x69x64ʺParser.Instance.Parse(input);
if (!_ʺx24x69x64ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx24x69x64ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx24x69x64ʺ(_ʺx24x69x64ʺ_1.Parsed), _ʺx24x69x64ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx69x64ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx69x64ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx69x64ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx69x64ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx69x64ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx69x64ʺParser.Instance.Parse(input);
if (!_ʺx69x64ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx69x64ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x64ʺⳆʺx69x64ʺ._ʺx69x64ʺ(_ʺx69x64ʺ_1.Parsed), _ʺx69x64ʺ_1.Remainder);
                }
            }
        }
    }
    
}
