namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x3BParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x3B> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x3B>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x3B> Parse(IInput<char>? input)
            {
                var _x3B = CombinatorParsingV2.Parse.Char((char)0x3B).Parse(input);
if (!_x3B.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x3B)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x3B.Instance, _x3B.Remainder);
            }
        }
    }
    
}
