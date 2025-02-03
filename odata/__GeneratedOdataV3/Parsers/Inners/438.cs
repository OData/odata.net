namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx49x6Ex74x33x32ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ> Parse(IInput<char>? input)
            {
                var _x49_1 = __GeneratedOdataV3.Parsers.Inners._x49Parser.Instance.Parse(input);
if (!_x49_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x49_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ)!, input);
}

var _x33_1 = __GeneratedOdataV3.Parsers.Inners._x33Parser.Instance.Parse(_x74_1.Remainder);
if (!_x33_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ)!, input);
}

var _x32_1 = __GeneratedOdataV3.Parsers.Inners._x32Parser.Instance.Parse(_x33_1.Remainder);
if (!_x32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx49x6Ex74x33x32ʺ.Instance, _x32_1.Remainder);
            }
        }
    }
    
}
