namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ> Instance { get; } = (_ʺx24x74x6Fx70ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ>(_ʺx74x6Fx70ʺParser.Instance);
        
        public static class _ʺx24x74x6Fx70ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx24x74x6Fx70ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx24x74x6Fx70ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx24x74x6Fx70ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x74x6Fx70ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x74x6Fx70ʺParser.Instance.Parse(input);
if (!_ʺx24x74x6Fx70ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx24x74x6Fx70ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx24x74x6Fx70ʺ(_ʺx24x74x6Fx70ʺ_1.Parsed), _ʺx24x74x6Fx70ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx74x6Fx70ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx74x6Fx70ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx74x6Fx70ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx74x6Fx70ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx74x6Fx70ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx74x6Fx70ʺParser.Instance.Parse(input);
if (!_ʺx74x6Fx70ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx74x6Fx70ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ._ʺx74x6Fx70ʺ(_ʺx74x6Fx70ʺ_1.Parsed), _ʺx74x6Fx70ʺ_1.Remainder);
                }
            }
        }
    }
    
}
