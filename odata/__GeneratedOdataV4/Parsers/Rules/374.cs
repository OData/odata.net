namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _COLONParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._COLON> Instance { get; } = (_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._COLON>(_ʺx25x33x41ʺParser.Instance);
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx3Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._COLON._ʺx3Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx3Aʺ.Instance, _ʺx3Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x33x41ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx25x33x41ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx25x33x41ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx25x33x41ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x33x41ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x33x41ʺParser.Instance.Parse(input);
if (!_ʺx25x33x41ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._COLON._ʺx25x33x41ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._COLON._ʺx25x33x41ʺ.Instance, _ʺx25x33x41ʺ_1.Remainder);
                }
            }
        }
    }
    
}
