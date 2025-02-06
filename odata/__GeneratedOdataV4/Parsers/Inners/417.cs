namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6EʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ> Parse(IInput<char>? input)
            {
                var _x43_1 = __GeneratedOdataV4.Parsers.Inners._x43Parser.Instance.Parse(input);
if (!_x43_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x43_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x6C_2 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x6C_1.Remainder);
if (!_x6C_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x6C_2.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x65_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x63_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x69_1.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x6F_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ.Instance, _x6E_1.Remainder);
            }
        }
    }
    
}
