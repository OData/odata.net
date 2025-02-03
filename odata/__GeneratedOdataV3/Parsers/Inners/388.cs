namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x35x44ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x35x44ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x35x44ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx25x35x44ʺ> Parse(IInput<char>? input)
            {
                var _x25_1 = __GeneratedOdataV3.Parsers.Inners._x25Parser.Instance.Parse(input);
if (!_x25_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x35x44ʺ)!, input);
}

var _x35_1 = __GeneratedOdataV3.Parsers.Inners._x35Parser.Instance.Parse(_x25_1.Remainder);
if (!_x35_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x35x44ʺ)!, input);
}

var _x44_1 = __GeneratedOdataV3.Parsers.Inners._x44Parser.Instance.Parse(_x35_1.Remainder);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx25x35x44ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx25x35x44ʺ.Instance, _x44_1.Remainder);
            }
        }
    }
    
}
