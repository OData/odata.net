namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x36Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x36> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x36>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x36> Parse(IInput<char>? input)
            {
                var _x36 = CombinatorParsingV2.Parse.Char((char)0x36).Parse(input);
if (!_x36.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x36)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x36.Instance, _x36.Remainder);
            }
        }
    }
    
}
