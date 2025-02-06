namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x6BParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6B> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6B>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x6B> Parse(IInput<char>? input)
            {
                var _x6B = CombinatorParsingV2.Parse.Char((char)0x6B).Parse(input);
if (!_x6B.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x6B)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x6B.Instance, _x6B.Remainder);
            }
        }
    }
    
}
