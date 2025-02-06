namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2CʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Cʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Cʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Cʺ> Parse(IInput<char>? input)
            {
                var _x2C_1 = __GeneratedOdataV4.Parsers.Inners._x2CParser.Instance.Parse(input);
if (!_x2C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx2Cʺ.Instance, _x2C_1.Remainder);
            }
        }
    }
    
}
