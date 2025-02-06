namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx29ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx29ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx29ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx29ʺ> Parse(IInput<char>? input)
            {
                var _x29_1 = __GeneratedOdataV4.Parsers.Inners._x29Parser.Instance.Parse(input);
if (!_x29_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx29ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx29ʺ.Instance, _x29_1.Remainder);
            }
        }
    }
    
}
