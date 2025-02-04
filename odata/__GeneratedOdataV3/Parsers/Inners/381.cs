namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5BʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Bʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Bʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Bʺ> Parse(IInput<char>? input)
            {
                var _x5B_1 = __GeneratedOdataV3.Parsers.Inners._x5BParser.Instance.Parse(input);
if (!_x5B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx5Bʺ.Instance, _x5B_1.Remainder);
            }
        }
    }
    
}
