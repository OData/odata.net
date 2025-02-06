namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ> Parse(IInput<char>? input)
            {
                var _x68_1 = __GeneratedOdataV4.Parsers.Inners._x68Parser.Instance.Parse(input);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x68_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x61_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x73_1.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV4.Parsers.Inners._x75Parser.Instance.Parse(_x73_2.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x62_1 = __GeneratedOdataV4.Parsers.Inners._x62Parser.Instance.Parse(_x75_1.Remainder);
if (!_x62_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x73_3 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x62_1.Remainder);
if (!_x73_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x73_3.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x71_1 = __GeneratedOdataV4.Parsers.Inners._x71Parser.Instance.Parse(_x65_1.Remainder);
if (!_x71_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x75_2 = __GeneratedOdataV4.Parsers.Inners._x75Parser.Instance.Parse(_x71_1.Remainder);
if (!_x75_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x75_2.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x65_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

var _x65_3 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x63_1.Remainder);
if (!_x65_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x71x75x65x6Ex63x65ʺ.Instance, _x65_3.Remainder);
            }
        }
    }
    
}
