namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ> Parse(IInput<char>? input)
            {
                var _x67_1 = __GeneratedOdataV3.Parsers.Inners._x67Parser.Instance.Parse(input);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x67_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x65_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x2E_1 = __GeneratedOdataV3.Parsers.Inners._x2EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV3.Parsers.Inners._x64Parser.Instance.Parse(_x2E_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x64_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x69_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x73_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x61_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x63_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ.Instance, _x65_2.Remainder);
            }
        }
    }
    
}
