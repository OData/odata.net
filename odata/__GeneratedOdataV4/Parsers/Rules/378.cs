namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SEMIParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._SEMI> Instance { get; } = (_ʺx3BʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._SEMI>(_ʺx25x33x42ʺParser.Instance);
        
        public static class _ʺx3BʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx3Bʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx3Bʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx3Bʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Bʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3BʺParser.Instance.Parse(input);
if (!_ʺx3Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx3Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx3Bʺ.Instance, _ʺx3Bʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x33x42ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx25x33x42ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx25x33x42ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx25x33x42ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x33x42ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x33x42ʺParser.Instance.Parse(input);
if (!_ʺx25x33x42ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx25x33x42ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._SEMI._ʺx25x33x42ʺ.Instance, _ʺx25x33x42ʺ_1.Remainder);
                }
            }
        }
    }
    
}
