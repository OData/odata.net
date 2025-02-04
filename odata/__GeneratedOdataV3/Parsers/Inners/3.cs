namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x73Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x73> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x73>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x73> Parse(IInput<char>? input)
            {
                var _x73 = CombinatorParsingV2.Parse.Char((char)0x73).Parse(input);
if (!_x73.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x73)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x73.Instance, _x73.Remainder);
            }
        }
    }
    
}
