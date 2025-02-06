namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3DʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Dʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Dʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Dʺ> Parse(IInput<char>? input)
            {
                var _x3D_1 = __GeneratedOdataV4.Parsers.Inners._x3DParser.Instance.Parse(input);
if (!_x3D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx3Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx3Dʺ.Instance, _x3D_1.Remainder);
            }
        }
    }
    
}
