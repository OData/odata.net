namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x29Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x29> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x29>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x29> Parse(IInput<char>? input)
            {
                var _x29 = CombinatorParsingV2.Parse.Char((char)0x29).Parse(input);
if (!_x29.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x29)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x29.Instance, _x29.Remainder);
            }
        }
    }
    
}
