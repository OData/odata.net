namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _STARParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._STAR> Instance { get; } = (_ʺx2AʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._STAR>(_ʺx25x32x41ʺParser.Instance);
        
        public static class _ʺx2AʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx2Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx2Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx2Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2AʺParser.Instance.Parse(input);
if (!_ʺx2Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._STAR._ʺx2Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx2Aʺ.Instance, _ʺx2Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x41ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx25x32x41ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx25x32x41ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx25x32x41ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x41ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x32x41ʺParser.Instance.Parse(input);
if (!_ʺx25x32x41ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._STAR._ʺx25x32x41ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._STAR._ʺx25x32x41ʺ.Instance, _ʺx25x32x41ʺ_1.Remainder);
                }
            }
        }
    }
    
}
