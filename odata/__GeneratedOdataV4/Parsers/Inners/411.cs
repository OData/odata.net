namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x30Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x30> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x30>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x30> Parse(IInput<char>? input)
            {
                var _x30 = CombinatorParsingV2.Parse.Char((char)0x30).Parse(input);
if (!_x30.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x30)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x30.Instance, _x30.Remainder);
            }
        }
    }
    
}
