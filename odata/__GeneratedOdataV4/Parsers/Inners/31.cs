namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x6Dx65x74x61x64x61x74x61ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ> Parse(IInput<char>? input)
            {
                var _x24_1 = __GeneratedOdataV4.Parsers.Inners._x24Parser.Instance.Parse(input);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV4.Parsers.Inners._x6DParser.Instance.Parse(_x24_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x65_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x61_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x61_2 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x64_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x61_2.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

var _x61_3 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x74_2.Remainder);
if (!_x61_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ.Instance, _x61_3.Remainder);
            }
        }
    }
    
}
