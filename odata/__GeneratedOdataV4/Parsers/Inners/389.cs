namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5DʺⳆʺx25x35x44ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ> Instance { get; } = (_ʺx5DʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ>(_ʺx25x35x44ʺParser.Instance);
        
        public static class _ʺx5DʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5DʺParser.Instance.Parse(input);
if (!_ʺx5Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ.Instance, _ʺx5Dʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x35x44ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x35x44ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x35x44ʺParser.Instance.Parse(input);
if (!_ʺx25x35x44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ.Instance, _ʺx25x35x44ʺ_1.Remainder);
                }
            }
        }
    }
    
}
