namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x37Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x37> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x37>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x37> Parse(IInput<char>? input)
            {
                var _x37 = CombinatorParsingV2.Parse.Char((char)0x37).Parse(input);
if (!_x37.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x37)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x37.Instance, _x37.Remainder);
            }
        }
    }
    
}
