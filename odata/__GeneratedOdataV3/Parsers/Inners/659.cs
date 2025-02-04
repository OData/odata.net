namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx7EʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx7Eʺ> Parse(IInput<char>? input)
            {
                var _x7E_1 = __GeneratedOdataV3.Parsers.Inners._x7EParser.Instance.Parse(input);
if (!_x7E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx7Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx7Eʺ.Instance, _x7E_1.Remainder);
            }
        }
    }
    
}
