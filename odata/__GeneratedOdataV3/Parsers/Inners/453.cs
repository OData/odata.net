namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ> Parse(IInput<char>? input)
            {
                var _x4D_1 = __GeneratedOdataV3.Parsers.Inners._x4DParser.Instance.Parse(input);
if (!_x4D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV3.Parsers.Inners._x75Parser.Instance.Parse(_x4D_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x75_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x4C_1 = __GeneratedOdataV3.Parsers.Inners._x4CParser.Instance.Parse(_x69_1.Remainder);
if (!_x4C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x69_2 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x4C_1.Remainder);
if (!_x69_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x53_1 = __GeneratedOdataV3.Parsers.Inners._x53Parser.Instance.Parse(_x65_1.Remainder);
if (!_x53_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x53_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x74_2.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x69_3 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x72_1.Remainder);
if (!_x69_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_3.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

var _x67_1 = __GeneratedOdataV3.Parsers.Inners._x67Parser.Instance.Parse(_x6E_2.Remainder);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ.Instance, _x67_1.Remainder);
            }
        }
    }
    
}
