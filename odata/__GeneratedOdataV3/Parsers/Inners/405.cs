namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx75ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx75ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx75ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx75ʺ> Parse(IInput<char>? input)
            {
                var _x75_1 = __GeneratedOdataV3.Parsers.Inners._x75Parser.Instance.Parse(input);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx75ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx75ʺ.Instance, _x75_1.Remainder);
            }
        }
    }
    
}
