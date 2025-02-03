namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Fx64x61x74x61x2EʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ> Parse(IInput<char>? input)
            {
                var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(input);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ)!, input);
}

var _x64_1 = __GeneratedOdataV3.Parsers.Inners._x64Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x64_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x61_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ)!, input);
}

var _x61_2 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ)!, input);
}

var _x2E_1 = __GeneratedOdataV3.Parsers.Inners._x2EParser.Instance.Parse(_x61_2.Remainder);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx64x61x74x61x2Eʺ.Instance, _x2E_1.Remainder);
            }
        }
    }
    
}
