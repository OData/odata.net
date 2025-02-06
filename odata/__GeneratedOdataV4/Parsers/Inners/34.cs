namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x6CParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6C> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6C>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x6C> Parse(IInput<char>? input)
            {
                var _x6C = CombinatorParsingV2.Parse.Char((char)0x6C).Parse(input);
if (!_x6C.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x6C)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x6C.Instance, _x6C.Remainder);
            }
        }
    }
    
}
