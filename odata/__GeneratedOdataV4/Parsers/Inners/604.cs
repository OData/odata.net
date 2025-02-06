namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx40ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx40ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx40ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx40ʺ> Parse(IInput<char>? input)
            {
                var _x40_1 = __GeneratedOdataV4.Parsers.Inners._x40Parser.Instance.Parse(input);
if (!_x40_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx40ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx40ʺ.Instance, _x40_1.Remainder);
            }
        }
    }
    
}
