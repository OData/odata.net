namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx33ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx33ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx33ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx33ʺ> Parse(IInput<char>? input)
            {
                var _x33_1 = __GeneratedOdataV3.Parsers.Inners._x33Parser.Instance.Parse(input);
if (!_x33_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx33ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx33ʺ.Instance, _x33_1.Remainder);
            }
        }
    }
    
}
