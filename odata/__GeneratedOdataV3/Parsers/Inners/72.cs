namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x26Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x26> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x26>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x26> Parse(IInput<char>? input)
            {
                var _x26 = CombinatorParsingV2.Parse.Char((char)0x26).Parse(input);
if (!_x26.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x26)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x26.Instance, _x26.Remainder);
            }
        }
    }
    
}
