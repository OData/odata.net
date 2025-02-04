namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5DʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Dʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Dʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Dʺ> Parse(IInput<char>? input)
            {
                var _x5D_1 = __GeneratedOdataV3.Parsers.Inners._x5DParser.Instance.Parse(input);
if (!_x5D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx5Dʺ.Instance, _x5D_1.Remainder);
            }
        }
    }
    
}
