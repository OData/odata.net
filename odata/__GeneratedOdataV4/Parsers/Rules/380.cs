namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SQUOTEParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE> Instance { get; } = (_ʺx27ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE>(_ʺx25x32x37ʺParser.Instance);
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx27ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx27ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx27ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx27ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx27ʺParser.Instance.Parse(input);
if (!_ʺx27ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx27ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx27ʺ.Instance, _ʺx27ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x37ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x37ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x32x37ʺParser.Instance.Parse(input);
if (!_ʺx25x32x37ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx25x32x37ʺ.Instance, _ʺx25x32x37ʺ_1.Remainder);
                }
            }
        }
    }
    
}
