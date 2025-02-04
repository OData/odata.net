namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x61Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x61> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x61>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x61> Parse(IInput<char>? input)
            {
                var _x61 = CombinatorParsingV2.Parse.Char((char)0x61).Parse(input);
if (!_x61.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x61)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x61.Instance, _x61.Remainder);
            }
        }
    }
    
}
