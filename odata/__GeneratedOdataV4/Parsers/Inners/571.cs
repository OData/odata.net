namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ> Parse(IInput<char>? input)
            {
                var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(input);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x61_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x6C_2 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x6C_1.Remainder);
if (!_x6C_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x6C_2.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x77_1 = __GeneratedOdataV4.Parsers.Inners._x77Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x77_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x2D_1 = __GeneratedOdataV4.Parsers.Inners._x2DParser.Instance.Parse(_x77_1.Remainder);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x2D_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x65_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x69_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV4.Parsers.Inners._x79Parser.Instance.Parse(_x74_2.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x79_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x72_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(_x65_2.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x65_3 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x66_1.Remainder);
if (!_x65_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x72_2 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x65_3.Remainder);
if (!_x72_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x65_4 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x72_2.Remainder);
if (!_x65_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x65_4.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x6E_2.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x65_5 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x63_1.Remainder);
if (!_x65_5.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x65_5.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ.Instance, _x73_1.Remainder);
            }
        }
    }
    
}
