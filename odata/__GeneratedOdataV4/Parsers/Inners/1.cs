namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x74Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x74> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x74>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x74> Parse(IInput<char>? input)
            {
                var _x74 = CombinatorParsingV2.Parse.Char((char)0x74).Parse(input);
if (!_x74.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x74)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x74.Instance, _x74.Remainder);
            }
        }
    }
    
}
