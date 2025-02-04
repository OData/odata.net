namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx42ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx42ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx42ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx42ʺ> Parse(IInput<char>? input)
            {
                var _x42_1 = __GeneratedOdataV3.Parsers.Inners._x42Parser.Instance.Parse(input);
if (!_x42_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx42ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx42ʺ.Instance, _x42_1.Remainder);
            }
        }
    }
    
}
