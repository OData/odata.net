namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5FʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5Fʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5Fʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx5Fʺ> Parse(IInput<char>? input)
            {
                var _x5F_1 = __GeneratedOdataV4.Parsers.Inners._x5FParser.Instance.Parse(input);
if (!_x5F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx5Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx5Fʺ.Instance, _x5F_1.Remainder);
            }
        }
    }
    
}
