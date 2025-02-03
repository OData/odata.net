namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Dx3DʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Dx3Dʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Dx3Dʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Dx3Dʺ> Parse(IInput<char>? input)
            {
                var _x3D_1 = __GeneratedOdataV3.Parsers.Inners._x3DParser.Instance.Parse(input);
if (!_x3D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Dx3Dʺ)!, input);
}

var _x3D_2 = __GeneratedOdataV3.Parsers.Inners._x3DParser.Instance.Parse(_x3D_1.Remainder);
if (!_x3D_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Dx3Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx3Dx3Dʺ.Instance, _x3D_2.Remainder);
            }
        }
    }
    
}
