namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6EʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ> Parse(IInput<char>? input)
            {
                var _x24_1 = __GeneratedOdataV3.Parsers.Inners._x24Parser.Instance.Parse(input);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x64_1 = __GeneratedOdataV3.Parsers.Inners._x64Parser.Instance.Parse(_x24_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x64_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x65_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x61_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x74_2.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x6B_1 = __GeneratedOdataV3.Parsers.Inners._x6BParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x6B_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x65_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ.Instance, _x6E_1.Remainder);
            }
        }
    }
    
}
