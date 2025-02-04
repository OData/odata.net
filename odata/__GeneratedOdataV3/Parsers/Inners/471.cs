namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x59Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x59> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x59>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x59> Parse(IInput<char>? input)
            {
                var _x59 = CombinatorParsingV2.Parse.Char((char)0x59).Parse(input);
if (!_x59.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x59)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x59.Instance, _x59.Remainder);
            }
        }
    }
    
}
