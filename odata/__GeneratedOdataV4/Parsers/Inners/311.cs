namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ> Parse(IInput<char>? input)
            {
                var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(input);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x74_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x74_2.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x61_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x6C_1.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(_x6F_2.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x66_2 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(_x66_1.Remainder);
if (!_x66_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x66_2.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x73_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x74_3 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x65_1.Remainder);
if (!_x74_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV4.Parsers.Inners._x6DParser.Instance.Parse(_x74_3.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV4.Parsers.Inners._x75Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x74_4 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x75_1.Remainder);
if (!_x74_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x74_4.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x65_2.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ.Instance, _x73_2.Remainder);
            }
        }
    }
    
}
