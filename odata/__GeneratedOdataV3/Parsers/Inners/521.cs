namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx39ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx39ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx39ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx39ʺ> Parse(IInput<char>? input)
            {
                var _x39_1 = __GeneratedOdataV3.Parsers.Inners._x39Parser.Instance.Parse(input);
if (!_x39_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx39ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx39ʺ.Instance, _x39_1.Remainder);
            }
        }
    }
    
}
