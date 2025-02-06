namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x35x43ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ> Parse(IInput<char>? input)
            {
                var _x25_1 = __GeneratedOdataV4.Parsers.Inners._x25Parser.Instance.Parse(input);
if (!_x25_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ)!, input);
}

var _x35_1 = __GeneratedOdataV4.Parsers.Inners._x35Parser.Instance.Parse(_x25_1.Remainder);
if (!_x35_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ)!, input);
}

var _x43_1 = __GeneratedOdataV4.Parsers.Inners._x43Parser.Instance.Parse(_x35_1.Remainder);
if (!_x43_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ.Instance, _x43_1.Remainder);
            }
        }
    }
    
}
