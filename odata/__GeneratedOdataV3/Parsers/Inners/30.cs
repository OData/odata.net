namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x64Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x64> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x64>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x64> Parse(IInput<char>? input)
            {
                var _x64 = CombinatorParsingV2.Parse.Char((char)0x64).Parse(input);
if (!_x64.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x64)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x64.Instance, _x64.Remainder);
            }
        }
    }
    
}
