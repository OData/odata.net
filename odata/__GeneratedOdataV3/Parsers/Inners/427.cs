namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ> Parse(IInput<char>? input)
            {
                var _x44_1 = __GeneratedOdataV3.Parsers.Inners._x44Parser.Instance.Parse(input);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x44_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x61_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x74_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x54_1 = __GeneratedOdataV3.Parsers.Inners._x54Parser.Instance.Parse(_x65_1.Remainder);
if (!_x54_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x54_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(_x69_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x4F_1 = __GeneratedOdataV3.Parsers.Inners._x4FParser.Instance.Parse(_x65_2.Remainder);
if (!_x4F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV3.Parsers.Inners._x66Parser.Instance.Parse(_x4F_1.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x66_2 = __GeneratedOdataV3.Parsers.Inners._x66Parser.Instance.Parse(_x66_1.Remainder);
if (!_x66_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x66_2.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x65_3 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x73_1.Remainder);
if (!_x65_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x65_3.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺ.Instance, _x74_2.Remainder);
            }
        }
    }
    
}
