namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Fx44x61x74x61x2DʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ> Parse(IInput<char>? input)
            {
                var _x4F_1 = __GeneratedOdataV4.Parsers.Inners._x4FParser.Instance.Parse(input);
if (!_x4F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ)!, input);
}

var _x44_1 = __GeneratedOdataV4.Parsers.Inners._x44Parser.Instance.Parse(_x4F_1.Remainder);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x44_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x61_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ)!, input);
}

var _x61_2 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ)!, input);
}

var _x2D_1 = __GeneratedOdataV4.Parsers.Inners._x2DParser.Instance.Parse(_x61_2.Remainder);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dʺ.Instance, _x2D_1.Remainder);
            }
        }
    }
    
}
