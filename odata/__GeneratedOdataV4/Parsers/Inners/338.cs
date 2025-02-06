namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx64x69x76ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx64x69x76ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx64x69x76ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx64x69x76ʺ> Parse(IInput<char>? input)
            {
                var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(input);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx64x69x76ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x64_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx64x69x76ʺ)!, input);
}

var _x76_1 = __GeneratedOdataV4.Parsers.Inners._x76Parser.Instance.Parse(_x69_1.Remainder);
if (!_x76_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx64x69x76ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx64x69x76ʺ.Instance, _x76_1.Remainder);
            }
        }
    }
    
}
