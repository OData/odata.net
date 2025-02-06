namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x32Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x32> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x32>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x32> Parse(IInput<char>? input)
            {
                var _x32 = CombinatorParsingV2.Parse.Char((char)0x32).Parse(input);
if (!_x32.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x32)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x32.Instance, _x32.Remainder);
            }
        }
    }
    
}
