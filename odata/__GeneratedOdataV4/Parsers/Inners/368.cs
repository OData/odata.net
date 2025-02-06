namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx7BʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx7Bʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx7Bʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx7Bʺ> Parse(IInput<char>? input)
            {
                var _x7B_1 = __GeneratedOdataV4.Parsers.Inners._x7BParser.Instance.Parse(input);
if (!_x7B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx7Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx7Bʺ.Instance, _x7B_1.Remainder);
            }
        }
    }
    
}
