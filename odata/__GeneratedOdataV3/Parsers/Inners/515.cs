namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx32ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx32ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx32ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx32ʺ> Parse(IInput<char>? input)
            {
                var _x32_1 = __GeneratedOdataV3.Parsers.Inners._x32Parser.Instance.Parse(input);
if (!_x32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx32ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx32ʺ.Instance, _x32_1.Remainder);
            }
        }
    }
    
}
