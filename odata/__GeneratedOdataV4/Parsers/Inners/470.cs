namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx55ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx55ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx55ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx55ʺ> Parse(IInput<char>? input)
            {
                var _x55_1 = __GeneratedOdataV4.Parsers.Inners._x55Parser.Instance.Parse(input);
if (!_x55_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx55ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx55ʺ.Instance, _x55_1.Remainder);
            }
        }
    }
    
}
