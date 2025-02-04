namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx7BʺⳆʺx25x37x42ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ> Instance { get; } = (_ʺx7BʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ>(_ʺx25x37x42ʺParser.Instance);
        
        public static class _ʺx7BʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ> Parse(IInput<char>? input)
                {
                    var _ʺx7Bʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx7BʺParser.Instance.Parse(input);
if (!_ʺx7Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ.Instance, _ʺx7Bʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x37x42ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x37x42ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x37x42ʺParser.Instance.Parse(input);
if (!_ʺx25x37x42ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ.Instance, _ʺx25x37x42ʺ_1.Remainder);
                }
            }
        }
    }
    
}
