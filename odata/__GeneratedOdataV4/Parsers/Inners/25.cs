namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x6EParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6E> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6E>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x6E> Parse(IInput<char>? input)
            {
                var _x6E = CombinatorParsingV2.Parse.Char((char)0x6E).Parse(input);
if (!_x6E.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x6E)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x6E.Instance, _x6E.Remainder);
            }
        }
    }
    
}
