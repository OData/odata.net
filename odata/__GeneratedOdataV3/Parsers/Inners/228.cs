namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x28Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x28> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x28>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x28> Parse(IInput<char>? input)
            {
                var _x28 = CombinatorParsingV2.Parse.Char((char)0x28).Parse(input);
if (!_x28.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x28)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x28.Instance, _x28.Remainder);
            }
        }
    }
    
}
