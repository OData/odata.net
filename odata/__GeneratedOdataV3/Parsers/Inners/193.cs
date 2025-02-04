namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x44Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x44> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x44>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x44> Parse(IInput<char>? input)
            {
                var _x44 = CombinatorParsingV2.Parse.Char((char)0x44).Parse(input);
if (!_x44.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x44)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x44.Instance, _x44.Remainder);
            }
        }
    }
    
}
