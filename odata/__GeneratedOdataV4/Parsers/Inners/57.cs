namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x76Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x76> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x76>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x76> Parse(IInput<char>? input)
            {
                var _x76 = CombinatorParsingV2.Parse.Char((char)0x76).Parse(input);
if (!_x76.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x76)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x76.Instance, _x76.Remainder);
            }
        }
    }
    
}
