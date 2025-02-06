namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x5BParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x5B> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x5B>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x5B> Parse(IInput<char>? input)
            {
                var _x5B = CombinatorParsingV2.Parse.Char((char)0x5B).Parse(input);
if (!_x5B.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x5B)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x5B.Instance, _x5B.Remainder);
            }
        }
    }
    
}
