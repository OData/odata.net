namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x39Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x39> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x39>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x39> Parse(IInput<char>? input)
            {
                var _x39 = CombinatorParsingV2.Parse.Char((char)0x39).Parse(input);
if (!_x39.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x39)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x39.Instance, _x39.Remainder);
            }
        }
    }
    
}
