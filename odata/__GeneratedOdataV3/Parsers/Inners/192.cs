namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x4EParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x4E> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x4E>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x4E> Parse(IInput<char>? input)
            {
                var _x4E = CombinatorParsingV2.Parse.Char((char)0x4E).Parse(input);
if (!_x4E.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x4E)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x4E.Instance, _x4E.Remainder);
            }
        }
    }
    
}
