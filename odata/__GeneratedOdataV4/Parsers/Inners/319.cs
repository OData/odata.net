namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ> Parse(IInput<char>? input)
            {
                var _x67_1 = __GeneratedOdataV4.Parsers.Inners._x67Parser.Instance.Parse(input);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x67_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x65_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x2E_1 = __GeneratedOdataV4.Parsers.Inners._x2EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x2E_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x65_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x67_2 = __GeneratedOdataV4.Parsers.Inners._x67Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x67_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x67_2.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

var _x68_1 = __GeneratedOdataV4.Parsers.Inners._x68Parser.Instance.Parse(_x74_1.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx67x65x6Fx2Ex6Cx65x6Ex67x74x68ʺ.Instance, _x68_1.Remainder);
            }
        }
    }
    
}
