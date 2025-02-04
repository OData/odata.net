namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx54ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx54ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx54ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx54ʺ> Parse(IInput<char>? input)
            {
                var _x54_1 = __GeneratedOdataV3.Parsers.Inners._x54Parser.Instance.Parse(input);
if (!_x54_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx54ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx54ʺ.Instance, _x54_1.Remainder);
            }
        }
    }
    
}
