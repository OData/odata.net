namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx72ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72ʺ> Parse(IInput<char>? input)
            {
                var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(input);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx72ʺ.Instance, _x72_1.Remainder);
            }
        }
    }
    
}
