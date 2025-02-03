namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ> Parse(IInput<char>? input)
            {
                var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(input);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x63_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV3.Parsers.Inners._x75Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV3.Parsers.Inners._x64Parser.Instance.Parse(_x75_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x64_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x2D_1 = __GeneratedOdataV3.Parsers.Inners._x2DParser.Instance.Parse(_x65_1.Remainder);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x2D_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x61_1.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x6E_3 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x6E_2.Remainder);
if (!_x6E_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x6E_3.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x61_2 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x61_2.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x69_2 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x74_2.Remainder);
if (!_x69_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x69_2.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x6E_4 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x6F_2.Remainder);
if (!_x6E_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x6E_4.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ.Instance, _x73_1.Remainder);
            }
        }
    }
    
}
