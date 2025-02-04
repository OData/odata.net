namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x53Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x53> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x53>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x53> Parse(IInput<char>? input)
            {
                var _x53 = CombinatorParsingV2.Parse.Char((char)0x53).Parse(input);
if (!_x53.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x53)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x53.Instance, _x53.Remainder);
            }
        }
    }
    
}
