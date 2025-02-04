namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x3FParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x3F> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x3F>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x3F> Parse(IInput<char>? input)
            {
                var _x3F = CombinatorParsingV2.Parse.Char((char)0x3F).Parse(input);
if (!_x3F.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x3F)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x3F.Instance, _x3F.Remainder);
            }
        }
    }
    
}
