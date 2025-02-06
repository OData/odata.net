namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3BʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Bʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Bʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Bʺ> Parse(IInput<char>? input)
            {
                var _x3B_1 = __GeneratedOdataV4.Parsers.Inners._x3BParser.Instance.Parse(input);
if (!_x3B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx3Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx3Bʺ.Instance, _x3B_1.Remainder);
            }
        }
    }
    
}
