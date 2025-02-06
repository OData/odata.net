namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ> Parse(IInput<char>? input)
            {
                var _x2F_1 = __GeneratedOdataV4.Parsers.Inners._x2FParser.Instance.Parse(input);
if (!_x2F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x24_1 = __GeneratedOdataV4.Parsers.Inners._x24Parser.Instance.Parse(_x2F_1.Remainder);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x24_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x64_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x65_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x65_2 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x65_2.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x65_3 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x74_1.Remainder);
if (!_x65_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x64_2 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x65_3.Remainder);
if (!_x64_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x4C_1 = __GeneratedOdataV4.Parsers.Inners._x4CParser.Instance.Parse(_x64_2.Remainder);
if (!_x4C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x4C_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

var _x6B_1 = __GeneratedOdataV4.Parsers.Inners._x6BParser.Instance.Parse(_x6E_1.Remainder);
if (!_x6B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6Bʺ.Instance, _x6B_1.Remainder);
            }
        }
    }
    
}
