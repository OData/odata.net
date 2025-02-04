namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x31Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x31> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x31>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x31> Parse(IInput<char>? input)
            {
                var _x31 = CombinatorParsingV2.Parse.Char((char)0x31).Parse(input);
if (!_x31.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x31)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x31.Instance, _x31.Remainder);
            }
        }
    }
    
}
