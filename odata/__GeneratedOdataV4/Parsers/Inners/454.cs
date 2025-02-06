namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x50Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x50> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x50>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x50> Parse(IInput<char>? input)
            {
                var _x50 = CombinatorParsingV2.Parse.Char((char)0x50).Parse(input);
if (!_x50.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x50)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x50.Instance, _x50.Remainder);
            }
        }
    }
    
}
