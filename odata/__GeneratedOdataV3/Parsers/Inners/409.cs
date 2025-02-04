namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5CʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Cʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Cʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5Cʺ> Parse(IInput<char>? input)
            {
                var _x5C_1 = __GeneratedOdataV3.Parsers.Inners._x5CParser.Instance.Parse(input);
if (!_x5C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx5Cʺ.Instance, _x5C_1.Remainder);
            }
        }
    }
    
}
