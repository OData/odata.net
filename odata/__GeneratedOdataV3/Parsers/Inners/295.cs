namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx73x74x61x72x74x73x77x69x74x68ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ> Parse(IInput<char>? input)
            {
                var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(input);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x73_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x61_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x72_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x74_2.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x77_1 = __GeneratedOdataV3.Parsers.Inners._x77Parser.Instance.Parse(_x73_2.Remainder);
if (!_x77_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x77_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x74_3 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x69_1.Remainder);
if (!_x74_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

var _x68_1 = __GeneratedOdataV3.Parsers.Inners._x68Parser.Instance.Parse(_x74_3.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺ.Instance, _x68_1.Remainder);
            }
        }
    }
    
}
