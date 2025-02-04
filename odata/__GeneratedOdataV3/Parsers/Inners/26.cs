namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x69Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x69> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x69>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x69> Parse(IInput<char>? input)
            {
                var _x69 = CombinatorParsingV2.Parse.Char((char)0x69).Parse(input);
if (!_x69.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x69)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x69.Instance, _x69.Remainder);
            }
        }
    }
    
}
