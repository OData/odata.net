namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx73x75x62x73x74x72x69x6Ex67ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ> Parse(IInput<char>? input)
            {
                var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(input);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV4.Parsers.Inners._x75Parser.Instance.Parse(_x73_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x62_1 = __GeneratedOdataV4.Parsers.Inners._x62Parser.Instance.Parse(_x75_1.Remainder);
if (!_x62_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x62_1.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x73_2.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x74_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x72_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

var _x67_1 = __GeneratedOdataV4.Parsers.Inners._x67Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺ.Instance, _x67_1.Remainder);
            }
        }
    }
    
}
