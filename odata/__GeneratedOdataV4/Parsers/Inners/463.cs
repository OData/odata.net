namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx41ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺ> Parse(IInput<char>? input)
            {
                var _x41_1 = __GeneratedOdataV4.Parsers.Inners._x41Parser.Instance.Parse(input);
if (!_x41_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺ.Instance, _x41_1.Remainder);
            }
        }
    }
    
}
