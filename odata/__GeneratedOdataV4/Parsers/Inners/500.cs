namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5AʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5Aʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5Aʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5Aʺ> Parse(IInput<char>? input)
            {
                var _x5A_1 = __GeneratedOdataV4.Parsers.Inners._x5AParser.Instance.Parse(input);
if (!_x5A_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx5Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx5Aʺ.Instance, _x5A_1.Remainder);
            }
        }
    }
    
}
