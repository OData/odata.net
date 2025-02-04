namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _COMMAParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA> Instance { get; } = (_ʺx2CʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._COMMA>(_ʺx25x32x43ʺParser.Instance);
        
        public static class _ʺx2CʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx2Cʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx2Cʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx2Cʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Cʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2CʺParser.Instance.Parse(input);
if (!_ʺx2Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx2Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx2Cʺ.Instance, _ʺx2Cʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x43ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx25x32x43ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx25x32x43ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx25x32x43ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x43ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x32x43ʺParser.Instance.Parse(input);
if (!_ʺx25x32x43ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx25x32x43ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._COMMA._ʺx25x32x43ʺ.Instance, _ʺx25x32x43ʺ_1.Remainder);
                }
            }
        }
    }
    
}
