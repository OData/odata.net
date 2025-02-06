namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x78Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x78> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x78>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x78> Parse(IInput<char>? input)
            {
                var _x78 = CombinatorParsingV2.Parse.Char((char)0x78).Parse(input);
if (!_x78.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x78)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x78.Instance, _x78.Remainder);
            }
        }
    }
    
}
