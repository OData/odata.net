namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx37ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx37ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx37ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx37ʺ> Parse(IInput<char>? input)
            {
                var _x37_1 = __GeneratedOdataV3.Parsers.Inners._x37Parser.Instance.Parse(input);
if (!_x37_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx37ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx37ʺ.Instance, _x37_1.Remainder);
            }
        }
    }
    
}
