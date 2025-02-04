namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx30ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ> Parse(IInput<char>? input)
            {
                var _x30_1 = __GeneratedOdataV3.Parsers.Inners._x30Parser.Instance.Parse(input);
if (!_x30_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ.Instance, _x30_1.Remainder);
            }
        }
    }
    
}
