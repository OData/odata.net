namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x55Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x55> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x55>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x55> Parse(IInput<char>? input)
            {
                var _x55 = CombinatorParsingV2.Parse.Char((char)0x55).Parse(input);
if (!_x55.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x55)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x55.Instance, _x55.Remainder);
            }
        }
    }
    
}
