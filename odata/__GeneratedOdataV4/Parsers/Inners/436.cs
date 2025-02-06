namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx49x6Ex74x31x36ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ> Parse(IInput<char>? input)
            {
                var _x49_1 = __GeneratedOdataV4.Parsers.Inners._x49Parser.Instance.Parse(input);
if (!_x49_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x49_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ)!, input);
}

var _x31_1 = __GeneratedOdataV4.Parsers.Inners._x31Parser.Instance.Parse(_x74_1.Remainder);
if (!_x31_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ)!, input);
}

var _x36_1 = __GeneratedOdataV4.Parsers.Inners._x36Parser.Instance.Parse(_x31_1.Remainder);
if (!_x36_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx49x6Ex74x31x36ʺ.Instance, _x36_1.Remainder);
            }
        }
    }
    
}
