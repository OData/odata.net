namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x2EParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x2E> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x2E>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x2E> Parse(IInput<char>? input)
            {
                var _x2E = CombinatorParsingV2.Parse.Char((char)0x2E).Parse(input);
if (!_x2E.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x2E)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x2E.Instance, _x2E.Remainder);
            }
        }
    }
    
}
