namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x32x46ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ> Parse(IInput<char>? input)
            {
                var _x25_1 = __GeneratedOdataV4.Parsers.Inners._x25Parser.Instance.Parse(input);
if (!_x25_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ)!, input);
}

var _x32_1 = __GeneratedOdataV4.Parsers.Inners._x32Parser.Instance.Parse(_x25_1.Remainder);
if (!_x32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ)!, input);
}

var _x46_1 = __GeneratedOdataV4.Parsers.Inners._x46Parser.Instance.Parse(_x32_1.Remainder);
if (!_x46_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x46ʺ.Instance, _x46_1.Remainder);
            }
        }
    }
    
}
