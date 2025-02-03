namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ> Parse(IInput<char>? input)
            {
                var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(input);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x74_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x72_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x61_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x6B_1 = __GeneratedOdataV3.Parsers.Inners._x6BParser.Instance.Parse(_x63_1.Remainder);
if (!_x6B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x2D_1 = __GeneratedOdataV3.Parsers.Inners._x2DParser.Instance.Parse(_x6B_1.Remainder);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x63_2 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x2D_1.Remainder);
if (!_x63_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x68_1 = __GeneratedOdataV3.Parsers.Inners._x68Parser.Instance.Parse(_x63_2.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x61_2 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x68_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x61_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x67_1 = __GeneratedOdataV3.Parsers.Inners._x67Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x67_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x65_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ.Instance, _x73_1.Remainder);
            }
        }
    }
    
}
