namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx27ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx27ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx27ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx27ʺ> Parse(IInput<char>? input)
            {
                var _x27_1 = __GeneratedOdataV3.Parsers.Inners._x27Parser.Instance.Parse(input);
if (!_x27_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx27ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx27ʺ.Instance, _x27_1.Remainder);
            }
        }
    }
    
}
