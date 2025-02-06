namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x65Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x65> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x65>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x65> Parse(IInput<char>? input)
            {
                var _x65 = CombinatorParsingV2.Parse.Char((char)0x65).Parse(input);
if (!_x65.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x65)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x65.Instance, _x65.Remainder);
            }
        }
    }
    
}
