namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x67Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x67> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x67>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x67> Parse(IInput<char>? input)
            {
                var _x67 = CombinatorParsingV2.Parse.Char((char)0x67).Parse(input);
if (!_x67.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x67)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x67.Instance, _x67.Remainder);
            }
        }
    }
    
}
