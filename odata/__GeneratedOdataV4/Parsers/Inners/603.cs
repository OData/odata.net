namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x40Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x40> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x40>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x40> Parse(IInput<char>? input)
            {
                var _x40 = CombinatorParsingV2.Parse.Char((char)0x40).Parse(input);
if (!_x40.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x40)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x40.Instance, _x40.Remainder);
            }
        }
    }
    
}
