namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x32x39ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x32x39ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x32x39ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x32x39ʺ> Parse(IInput<char>? input)
            {
                var _x25_1 = __GeneratedOdataV3.Parsers.Inners._x25Parser.Instance.Parse(input);
if (!_x25_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x32x39ʺ)!, input);
}

var _x32_1 = __GeneratedOdataV3.Parsers.Inners._x32Parser.Instance.Parse(_x25_1.Remainder);
if (!_x32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x32x39ʺ)!, input);
}

var _x39_1 = __GeneratedOdataV3.Parsers.Inners._x39Parser.Instance.Parse(_x32_1.Remainder);
if (!_x39_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x32x39ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx25x32x39ʺ.Instance, _x39_1.Remainder);
            }
        }
    }
    
}
