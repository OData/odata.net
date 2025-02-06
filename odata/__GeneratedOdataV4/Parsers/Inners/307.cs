namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ> Parse(IInput<char>? input)
            {
                var _x66_1 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(input);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x66_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x72_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x61_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x63_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x69_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x61_2 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x61_2.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x73_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x63_2 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x65_1.Remainder);
if (!_x63_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x63_2.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x6F_2.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x6E_2.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x64_1.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ.Instance, _x73_2.Remainder);
            }
        }
    }
    
}
