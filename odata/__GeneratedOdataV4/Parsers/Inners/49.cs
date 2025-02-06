namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x66Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x66> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x66>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x66> Parse(IInput<char>? input)
            {
                var _x66 = CombinatorParsingV2.Parse.Char((char)0x66).Parse(input);
if (!_x66.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x66)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x66.Instance, _x66.Remainder);
            }
        }
    }
    
}
