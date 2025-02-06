namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx69x6EʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Eʺ> Parse(IInput<char>? input)
            {
                var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(input);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Eʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx69x6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx69x6Eʺ.Instance, _x6E_1.Remainder);
            }
        }
    }
    
}
