namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x2FParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x2F> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x2F>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x2F> Parse(IInput<char>? input)
            {
                var _x2F = CombinatorParsingV2.Parse.Char((char)0x2F).Parse(input);
if (!_x2F.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x2F)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x2F.Instance, _x2F.Remainder);
            }
        }
    }
    
}
