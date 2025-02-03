namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ> Parse(IInput<char>? input)
            {
                var _x43_1 = __GeneratedOdataV3.Parsers.Inners._x43Parser.Instance.Parse(input);
if (!_x43_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x43_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x6C_2 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x6C_1.Remainder);
if (!_x6C_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x6C_2.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x65_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x63_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x69_1.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x6F_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x28_1 = __GeneratedOdataV3.Parsers.Inners._x28Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x28_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x45_1 = __GeneratedOdataV3.Parsers.Inners._x45Parser.Instance.Parse(_x28_1.Remainder);
if (!_x45_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV3.Parsers.Inners._x64Parser.Instance.Parse(_x45_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(_x64_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x2E_1 = __GeneratedOdataV3.Parsers.Inners._x2EParser.Instance.Parse(_x6D_1.Remainder);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x45_2 = __GeneratedOdataV3.Parsers.Inners._x45Parser.Instance.Parse(_x2E_1.Remainder);
if (!_x45_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x45_2.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6E_2.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x69_2 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x74_2.Remainder);
if (!_x69_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x74_3 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x69_2.Remainder);
if (!_x74_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV3.Parsers.Inners._x79Parser.Instance.Parse(_x74_3.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x54_1 = __GeneratedOdataV3.Parsers.Inners._x54Parser.Instance.Parse(_x79_1.Remainder);
if (!_x54_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x79_2 = __GeneratedOdataV3.Parsers.Inners._x79Parser.Instance.Parse(_x54_1.Remainder);
if (!_x79_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV3.Parsers.Inners._x70Parser.Instance.Parse(_x79_2.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x70_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

var _x29_1 = __GeneratedOdataV3.Parsers.Inners._x29Parser.Instance.Parse(_x65_2.Remainder);
if (!_x29_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ.Instance, _x29_1.Remainder);
            }
        }
    }
    
}
