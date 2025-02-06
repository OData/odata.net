namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx73x6Bx69x70ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ> Parse(IInput<char>? input)
            {
                var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(input);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ)!, input);
}

var _x6B_1 = __GeneratedOdataV4.Parsers.Inners._x6BParser.Instance.Parse(_x73_1.Remainder);
if (!_x6B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x6B_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV4.Parsers.Inners._x70Parser.Instance.Parse(_x69_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx73x6Bx69x70ʺ.Instance, _x70_1.Remainder);
            }
        }
    }
    
}
