namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx7DʺⳆʺx25x37x44ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ> Instance { get; } = (_ʺx7DʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ>(_ʺx25x37x44ʺParser.Instance);
        
        public static class _ʺx7DʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx7Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx7DʺParser.Instance.Parse(input);
if (!_ʺx7Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ.Instance, _ʺx7Dʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x37x44ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x37x44ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x37x44ʺParser.Instance.Parse(input);
if (!_ʺx25x37x44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ.Instance, _ʺx25x37x44ʺ_1.Remainder);
                }
            }
        }
    }
    
}
