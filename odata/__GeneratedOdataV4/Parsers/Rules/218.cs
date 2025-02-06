namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _escapeParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._escape> Instance { get; } = (_ʺx5CʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._escape>(_ʺx25x35x43ʺParser.Instance);
        
        public static class _ʺx5CʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx5Cʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx5Cʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx5Cʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Cʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5CʺParser.Instance.Parse(input);
if (!_ʺx5Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._escape._ʺx5Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx5Cʺ.Instance, _ʺx5Cʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x35x43ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx25x35x43ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx25x35x43ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx25x35x43ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x35x43ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x35x43ʺParser.Instance.Parse(input);
if (!_ʺx25x35x43ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._escape._ʺx25x35x43ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._escape._ʺx25x35x43ʺ.Instance, _ʺx25x35x43ʺ_1.Remainder);
                }
            }
        }
    }
    
}
