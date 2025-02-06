namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx35ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx35ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx35ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx35ʺ> Parse(IInput<char>? input)
            {
                var _x35_1 = __GeneratedOdataV4.Parsers.Inners._x35Parser.Instance.Parse(input);
if (!_x35_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx35ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx35ʺ.Instance, _x35_1.Remainder);
            }
        }
    }
    
}
