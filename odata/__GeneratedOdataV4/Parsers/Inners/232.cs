namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x45Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x45> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x45>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x45> Parse(IInput<char>? input)
            {
                var _x45 = CombinatorParsingV2.Parse.Char((char)0x45).Parse(input);
if (!_x45.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x45)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x45.Instance, _x45.Remainder);
            }
        }
    }
    
}
