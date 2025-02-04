namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SIGNParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._SIGN> Instance { get; } = (_ʺx2BʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._SIGN>(_ʺx25x32x42ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._SIGN>(_ʺx2DʺParser.Instance);
        
        public static class _ʺx2BʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Bʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Bʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Bʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Bʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2BʺParser.Instance.Parse(input);
if (!_ʺx2Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Bʺ.Instance, _ʺx2Bʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x42ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx25x32x42ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx25x32x42ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx25x32x42ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x42ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x32x42ʺParser.Instance.Parse(input);
if (!_ʺx25x32x42ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx25x32x42ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx25x32x42ʺ.Instance, _ʺx25x32x42ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2DʺParser.Instance.Parse(input);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._SIGN._ʺx2Dʺ.Instance, _ʺx2Dʺ_1.Remainder);
                }
            }
        }
    }
    
}
