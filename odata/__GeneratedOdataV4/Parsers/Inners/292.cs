namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx69x6Ex64x65x78x6Fx66ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ> Parse(IInput<char>? input)
            {
                var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(input);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x64_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ)!, input);
}

var _x78_1 = __GeneratedOdataV4.Parsers.Inners._x78Parser.Instance.Parse(_x65_1.Remainder);
if (!_x78_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x78_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Ex64x65x78x6Fx66ʺ.Instance, _x66_1.Remainder);
            }
        }
    }
    
}
