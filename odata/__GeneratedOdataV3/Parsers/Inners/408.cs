namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x5CParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x5C> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x5C>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x5C> Parse(IInput<char>? input)
            {
                var _x5C = CombinatorParsingV2.Parse.Char((char)0x5C).Parse(input);
if (!_x5C.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x5C)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x5C.Instance, _x5C.Remainder);
            }
        }
    }
    
}
