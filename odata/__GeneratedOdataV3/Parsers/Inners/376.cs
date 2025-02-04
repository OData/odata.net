namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx7DʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7Dʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7Dʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7Dʺ> Parse(IInput<char>? input)
            {
                var _x7D_1 = __GeneratedOdataV3.Parsers.Inners._x7DParser.Instance.Parse(input);
if (!_x7D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx7Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx7Dʺ.Instance, _x7D_1.Remainder);
            }
        }
    }
    
}
