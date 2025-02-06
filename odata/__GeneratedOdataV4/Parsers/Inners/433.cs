namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x49Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x49> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x49>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x49> Parse(IInput<char>? input)
            {
                var _x49 = CombinatorParsingV2.Parse.Char((char)0x49).Parse(input);
if (!_x49.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x49)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x49.Instance, _x49.Remainder);
            }
        }
    }
    
}
