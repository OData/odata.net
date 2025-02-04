namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4DʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dʺ> Parse(IInput<char>? input)
            {
                var _x4D_1 = __GeneratedOdataV3.Parsers.Inners._x4DParser.Instance.Parse(input);
if (!_x4D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dʺ.Instance, _x4D_1.Remainder);
            }
        }
    }
    
}
