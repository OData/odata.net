namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx61x64x64ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x64x64ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x64x64ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x64x64ʺ> Parse(IInput<char>? input)
            {
                var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(input);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x64x64ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x61_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x64x64ʺ)!, input);
}

var _x64_2 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x64_1.Remainder);
if (!_x64_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x64x64ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx61x64x64ʺ.Instance, _x64_2.Remainder);
            }
        }
    }
    
}
