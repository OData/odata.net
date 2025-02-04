namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ> Parse(IInput<char>? input)
            {
                var _x26_1 = __GeneratedOdataV3.Parsers.Inners._x26Parser.Instance.Parse(input);
if (!_x26_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ.Instance, _x26_1.Remainder);
            }
        }
    }
    
}
