namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x52Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x52> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x52>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x52> Parse(IInput<char>? input)
            {
                var _x52 = CombinatorParsingV2.Parse.Char((char)0x52).Parse(input);
if (!_x52.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x52)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x52.Instance, _x52.Remainder);
            }
        }
    }
    
}
