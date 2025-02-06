namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x63Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x63> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x63>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x63> Parse(IInput<char>? input)
            {
                var _x63 = CombinatorParsingV2.Parse.Char((char)0x63).Parse(input);
if (!_x63.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x63)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x63.Instance, _x63.Remainder);
            }
        }
    }
    
}
