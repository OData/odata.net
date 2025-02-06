namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x51Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x51> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x51>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x51> Parse(IInput<char>? input)
            {
                var _x51 = CombinatorParsingV2.Parse.Char((char)0x51).Parse(input);
if (!_x51.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x51)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x51.Instance, _x51.Remainder);
            }
        }
    }
    
}
