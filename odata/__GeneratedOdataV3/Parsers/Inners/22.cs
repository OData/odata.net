namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3FʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ> Parse(IInput<char>? input)
            {
                var _x3F_1 = __GeneratedOdataV3.Parsers.Inners._x3FParser.Instance.Parse(input);
if (!_x3F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ.Instance, _x3F_1.Remainder);
            }
        }
    }
    
}
