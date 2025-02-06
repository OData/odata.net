namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx23ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ> Parse(IInput<char>? input)
            {
                var _x23_1 = __GeneratedOdataV4.Parsers.Inners._x23Parser.Instance.Parse(input);
if (!_x23_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ.Instance, _x23_1.Remainder);
            }
        }
    }
    
}
