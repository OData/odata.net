namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx53x52x49x44ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ> Parse(IInput<char>? input)
            {
                var _x53_1 = __GeneratedOdataV3.Parsers.Inners._x53Parser.Instance.Parse(input);
if (!_x53_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ)!, input);
}

var _x52_1 = __GeneratedOdataV3.Parsers.Inners._x52Parser.Instance.Parse(_x53_1.Remainder);
if (!_x52_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ)!, input);
}

var _x49_1 = __GeneratedOdataV3.Parsers.Inners._x49Parser.Instance.Parse(_x52_1.Remainder);
if (!_x49_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ)!, input);
}

var _x44_1 = __GeneratedOdataV3.Parsers.Inners._x44Parser.Instance.Parse(_x49_1.Remainder);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx53x52x49x44ʺ.Instance, _x44_1.Remainder);
            }
        }
    }
    
}
