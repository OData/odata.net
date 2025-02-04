namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x41Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x41> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x41>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x41> Parse(IInput<char>? input)
            {
                var _x41 = CombinatorParsingV2.Parse.Char((char)0x41).Parse(input);
if (!_x41.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x41)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x41.Instance, _x41.Remainder);
            }
        }
    }
    
}
