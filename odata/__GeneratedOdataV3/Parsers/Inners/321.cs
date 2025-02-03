namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx68x61x73x73x75x62x73x65x74ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ> Parse(IInput<char>? input)
            {
                var _x68_1 = __GeneratedOdataV3.Parsers.Inners._x68Parser.Instance.Parse(input);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x68_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x61_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x73_1.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV3.Parsers.Inners._x75Parser.Instance.Parse(_x73_2.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x62_1 = __GeneratedOdataV3.Parsers.Inners._x62Parser.Instance.Parse(_x75_1.Remainder);
if (!_x62_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x73_3 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x62_1.Remainder);
if (!_x73_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x73_3.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x65_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx68x61x73x73x75x62x73x65x74ʺ.Instance, _x74_1.Remainder);
            }
        }
    }
    
}
