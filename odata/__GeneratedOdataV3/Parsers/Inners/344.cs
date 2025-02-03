namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx69x73x6Fx66ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ> Parse(IInput<char>? input)
            {
                var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(input);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x69_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x73_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV3.Parsers.Inners._x66Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx69x73x6Fx66ʺ.Instance, _x66_1.Remainder);
            }
        }
    }
    
}
