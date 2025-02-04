namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x33Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x33> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x33>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x33> Parse(IInput<char>? input)
            {
                var _x33 = CombinatorParsingV2.Parse.Char((char)0x33).Parse(input);
if (!_x33.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x33)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x33.Instance, _x33.Remainder);
            }
        }
    }
    
}
