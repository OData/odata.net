namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx43ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx43ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx43ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx43ʺ> Parse(IInput<char>? input)
            {
                var _x43_1 = __GeneratedOdataV4.Parsers.Inners._x43Parser.Instance.Parse(input);
if (!_x43_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx43ʺ.Instance, _x43_1.Remainder);
            }
        }
    }
    
}
