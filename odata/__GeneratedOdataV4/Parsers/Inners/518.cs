namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx36ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx36ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx36ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx36ʺ> Parse(IInput<char>? input)
            {
                var _x36_1 = __GeneratedOdataV4.Parsers.Inners._x36Parser.Instance.Parse(input);
if (!_x36_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx36ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx36ʺ.Instance, _x36_1.Remainder);
            }
        }
    }
    
}
