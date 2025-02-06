namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx45x64x6Dx2EʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ> Parse(IInput<char>? input)
            {
                var _x45_1 = __GeneratedOdataV4.Parsers.Inners._x45Parser.Instance.Parse(input);
if (!_x45_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x45_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV4.Parsers.Inners._x6DParser.Instance.Parse(_x64_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ)!, input);
}

var _x2E_1 = __GeneratedOdataV4.Parsers.Inners._x2EParser.Instance.Parse(_x6D_1.Remainder);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx45x64x6Dx2Eʺ.Instance, _x2E_1.Remainder);
            }
        }
    }
    
}
