namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x42Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x42> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x42>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x42> Parse(IInput<char>? input)
            {
                var _x42 = CombinatorParsingV2.Parse.Char((char)0x42).Parse(input);
if (!_x42.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x42)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x42.Instance, _x42.Remainder);
            }
        }
    }
    
}
