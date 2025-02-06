namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x4CParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x4C> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x4C>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x4C> Parse(IInput<char>? input)
            {
                var _x4C = CombinatorParsingV2.Parse.Char((char)0x4C).Parse(input);
if (!_x4C.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x4C)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x4C.Instance, _x4C.Remainder);
            }
        }
    }
    
}
