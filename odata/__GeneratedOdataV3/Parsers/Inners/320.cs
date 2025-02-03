namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ> Parse(IInput<char>? input)
            {
                var _x67_1 = __GeneratedOdataV3.Parsers.Inners._x67Parser.Instance.Parse(input);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x67_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x65_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x2E_1 = __GeneratedOdataV3.Parsers.Inners._x2EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x2E_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x74_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x65_2.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x72_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x65_3 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x73_1.Remainder);
if (!_x65_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x65_3.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x63_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x74_2.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex69x6Ex74x65x72x73x65x63x74x73ʺ.Instance, _x73_2.Remainder);
            }
        }
    }
    
}
