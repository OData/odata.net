namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx65x6Ex64x73x77x69x74x68ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ> Parse(IInput<char>? input)
            {
                var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(input);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x65_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x64_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

var _x77_1 = __GeneratedOdataV4.Parsers.Inners._x77Parser.Instance.Parse(_x73_1.Remainder);
if (!_x77_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x77_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x69_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

var _x68_1 = __GeneratedOdataV4.Parsers.Inners._x68Parser.Instance.Parse(_x74_1.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx65x6Ex64x73x77x69x74x68ʺ.Instance, _x68_1.Remainder);
            }
        }
    }
    
}
