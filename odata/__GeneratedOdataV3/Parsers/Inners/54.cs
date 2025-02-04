namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x75Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x75> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x75>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x75> Parse(IInput<char>? input)
            {
                var _x75 = CombinatorParsingV2.Parse.Char((char)0x75).Parse(input);
if (!_x75.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x75)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x75.Instance, _x75.Remainder);
            }
        }
    }
    
}
