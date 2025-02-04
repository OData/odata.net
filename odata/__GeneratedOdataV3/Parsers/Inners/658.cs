namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x7EParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x7E> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x7E>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x7E> Parse(IInput<char>? input)
            {
                var _x7E = CombinatorParsingV2.Parse.Char((char)0x7E).Parse(input);
if (!_x7E.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x7E)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x7E.Instance, _x7E.Remainder);
            }
        }
    }
    
}
