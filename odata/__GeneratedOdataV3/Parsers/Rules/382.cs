namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE> Instance { get; } = (_ʺx29ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE>(_ʺx25x32x39ʺParser.Instance);
        
        public static class _ʺx29ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx29ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx29ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx29ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx29ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx29ʺParser.Instance.Parse(input);
if (!_ʺx29ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx29ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx29ʺ(_ʺx29ʺ_1.Parsed), _ʺx29ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x39ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x39ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x32x39ʺParser.Instance.Parse(input);
if (!_ʺx25x32x39ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._CLOSE._ʺx25x32x39ʺ(_ʺx25x32x39ʺ_1.Parsed), _ʺx25x32x39ʺ_1.Remainder);
                }
            }
        }
    }
    
}
