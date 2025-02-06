namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx74ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74ʺ> Parse(IInput<char>? input)
            {
                var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(input);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx74ʺ.Instance, _x74_1.Remainder);
            }
        }
    }
    
}
