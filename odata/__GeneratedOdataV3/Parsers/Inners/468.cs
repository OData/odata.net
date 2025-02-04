namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx51ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx51ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx51ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx51ʺ> Parse(IInput<char>? input)
            {
                var _x51_1 = __GeneratedOdataV3.Parsers.Inners._x51Parser.Instance.Parse(input);
if (!_x51_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx51ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx51ʺ.Instance, _x51_1.Remainder);
            }
        }
    }
    
}
