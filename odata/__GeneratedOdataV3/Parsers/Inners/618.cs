namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx28ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx28ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx28ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx28ʺ> Parse(IInput<char>? input)
            {
                var _x28_1 = __GeneratedOdataV3.Parsers.Inners._x28Parser.Instance.Parse(input);
if (!_x28_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx28ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx28ʺ.Instance, _x28_1.Remainder);
            }
        }
    }
    
}
