namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x47Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x47> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x47>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x47> Parse(IInput<char>? input)
            {
                var _x47 = CombinatorParsingV2.Parse.Char((char)0x47).Parse(input);
if (!_x47.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x47)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x47.Instance, _x47.Remainder);
            }
        }
    }
    
}
