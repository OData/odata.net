namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x33x42ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x33x42ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x33x42ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x33x42ʺ> Parse(IInput<char>? input)
            {
                var _x25_1 = __GeneratedOdataV3.Parsers.Inners._x25Parser.Instance.Parse(input);
if (!_x25_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x33x42ʺ)!, input);
}

var _x33_1 = __GeneratedOdataV3.Parsers.Inners._x33Parser.Instance.Parse(_x25_1.Remainder);
if (!_x33_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x33x42ʺ)!, input);
}

var _x42_1 = __GeneratedOdataV3.Parsers.Inners._x42Parser.Instance.Parse(_x33_1.Remainder);
if (!_x42_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x33x42ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx25x33x42ʺ.Instance, _x42_1.Remainder);
            }
        }
    }
    
}
