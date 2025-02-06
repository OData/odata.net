namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3AʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ> Parse(IInput<char>? input)
            {
                var _x3A_1 = __GeneratedOdataV4.Parsers.Inners._x3AParser.Instance.Parse(input);
if (!_x3A_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ.Instance, _x3A_1.Remainder);
            }
        }
    }
    
}
