namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5BʺⳆʺx25x35x42ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ> Instance { get; } = (_ʺx5BʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ>(_ʺx25x35x42ʺParser.Instance);
        
        public static class _ʺx5BʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Bʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx5BʺParser.Instance.Parse(input);
if (!_ʺx5Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ(_ʺx5Bʺ_1.Parsed), _ʺx5Bʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x35x42ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x35x42ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x35x42ʺParser.Instance.Parse(input);
if (!_ʺx25x35x42ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ(_ʺx25x35x42ʺ_1.Parsed), _ʺx25x35x42ʺ_1.Remainder);
                }
            }
        }
    }
    
}
