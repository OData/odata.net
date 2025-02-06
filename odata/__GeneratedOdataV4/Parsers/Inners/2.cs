namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x70Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x70> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x70>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x70> Parse(IInput<char>? input)
            {
                var _x70 = CombinatorParsingV2.Parse.Char((char)0x70).Parse(input);
if (!_x70.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x70)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x70.Instance, _x70.Remainder);
            }
        }
    }
    
}
