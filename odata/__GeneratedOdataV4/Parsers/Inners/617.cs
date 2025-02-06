namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x32x37ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x37ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x37ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x37ʺ> Parse(IInput<char>? input)
            {
                var _x25_1 = __GeneratedOdataV4.Parsers.Inners._x25Parser.Instance.Parse(input);
if (!_x25_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x32x37ʺ)!, input);
}

var _x32_1 = __GeneratedOdataV4.Parsers.Inners._x32Parser.Instance.Parse(_x25_1.Remainder);
if (!_x32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x32x37ʺ)!, input);
}

var _x37_1 = __GeneratedOdataV4.Parsers.Inners._x37Parser.Instance.Parse(_x32_1.Remainder);
if (!_x37_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x32x37ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x37ʺ.Instance, _x37_1.Remainder);
            }
        }
    }
    
}
